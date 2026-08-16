using HotelListing.Api.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelListing.Api.Tests.Integration.Config
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
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

