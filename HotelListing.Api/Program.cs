using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HotelListing.Api.Handler;
using HotelListing.Api.Common.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelListing.Api.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;
using HotelListing.Api.Authorization.Requirements;
using HotelListing.Api.Domain;
using HotelListing.Api.Application.Services;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Common.Models.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");


if (!builder.Environment.IsEnvironment("Development"))
{
    builder.Services.AddDbContextPool<HotelListingDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
            sqlOptions.MigrationsAssembly("HotelListing.Api.Domain")), poolSize: 128);
}


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
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

builder.Services.AddControllers()
    .AddNewtonsoftJson()
    .AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }