using HotelListing.Api.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelListing.Api.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                // change connection parameters to test db

                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<HotelListingDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }


                var connectionString = context.Configuration.GetConnectionString("HotelListingDbConnectionString") ??
                throw new InvalidOperationException("Connection string 'HotelListingDbConnectionString' not found.");

                var connectionBuilder = new SqlConnectionStringBuilder(connectionString)
                {
                    InitialCatalog = "HotelListingTestDb"
                };

                services.AddDbContext<HotelListingDbContext>(options =>
                {
                    options.UseSqlServer(
                        connectionBuilder.ConnectionString);
                });


            });
        }

        public async Task InitializeAsync()
        {
            // create test db

            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HotelListingDbContext>();

            await context.Database.EnsureDeletedAsync();

            await context.Database.MigrateAsync();

            
            await TestDatabaseSeeder.SeedAsync(
                scope.ServiceProvider);
            

        }

        public Task DisposeAsync()
        {
            // run after test completion

            return Task.CompletedTask;
        }



            
    }
}

