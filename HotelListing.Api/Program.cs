using HotelListing.Api.Application.Caching;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.Services;
using HotelListing.Api.Authorization.Handlers;
using HotelListing.Api.Authorization.Requirements;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Models.Config;
using HotelListing.Api.Domain;
using HotelListing.Api.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting HotelListing API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
    );

    if (builder.Environment.IsEnvironment("Testing"))
    {
        builder.Configuration.AddUserSecrets<Program>();
    }

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

    var connectionBuilder = new SqlConnectionStringBuilder(connectionString);

    if (builder.Environment.IsEnvironment("Testing"))
    {
        connectionBuilder.InitialCatalog = "HotelListingTestDb";
    }


    builder.Services.AddDbContextPool<HotelListingDbContext>(options =>
    {
        options.UseSqlServer(connectionBuilder.ConnectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("HotelListing.Api.Domain");
            sqlOptions.CommandTimeout(30);
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
                );
        });
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }

    }, poolSize: 128);





    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

    if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    {
        Log.Fatal("JwtSettings:Key is not configured");
        throw new InvalidOperationException("JwtSettings:Key is not configured");
    }

    builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<HotelListingDbContext>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationDefaults.ApiKeyScheme, options => { });

    builder.Services.AddAuthorization();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ManageHotel", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.Requirements.Add(new ManageHotelRequirement());
        });
    });

    builder.Services.AddScoped<IAuthorizationHandler, ManageHotelAuthorizationHandler>();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<ICountryService, CountryService>();

    builder.Services.AddScoped<IHotelService, HotelService>();

    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddScoped<IBookingService, BookingService>();

    builder.Services.AddScoped<IApiKeyValidatorService, ApiKeyValidatorService>();

    builder.Services.AddSingleton<MemoryCacheService>();

    builder.Services.AddControllers()
        .AddNewtonsoftJson()
        .AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("fixed", opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 100;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 50;
        });

        options.AddPolicy("perUser", context =>
        {
            var username = context.User?.Identity?.Name ?? "anonymous";

            return RateLimitPartition.GetSlidingWindowLimiter(username, _ => new SlidingWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 50,
                SegmentsPerWindow = 6,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 3
            });
        });

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 200,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            });
        });

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.OnRejected = async (context, cancellationToken) =>
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
            }

            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";

            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Too many requests",
                message = "Rate limit exceeded. Please try again later.",
                retryAfter = retryAfter.TotalSeconds
            }, cancellationToken: cancellationToken);
        };
    });

    builder.Services.AddMemoryCache();

    var app = builder.Build();

    app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseRateLimiter();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("HotelListing API started successfully");

    app.Run();


}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }