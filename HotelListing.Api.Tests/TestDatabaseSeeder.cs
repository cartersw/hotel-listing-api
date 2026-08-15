using HotelListing.Api.Common.Constants;
using HotelListing.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelListing.Api.Tests
{
    internal class TestDatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context =
                services.GetRequiredService<HotelListingDbContext>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            await SeedCountriesAndHotelsAsync(context);
        }

        private static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {

        }

        private static async Task SeedUsersAsync(
            UserManager<ApplicationUser> userManager)
        {

            var admin = new ApplicationUser
            {
                Id = TestUsers.AdminId,
                UserName = TestUsers.AdminUserName,
                FirstName = TestUsers.AdminUserName,
                LastName = TestUsers.AdminUserName,
                Email = TestUsers.AdminEmail
            };

            var hotelAdmin = new ApplicationUser
            {
                Id = TestUsers.ManagerOneId,
                UserName = TestUsers.ManagerOneUserName,
                FirstName = TestUsers.ManagerOneUserName,
                LastName = TestUsers.ManagerOneUserName,
                Email = TestUsers.ManagerOneEmail
            };

            var user = new ApplicationUser
            {
                Id = TestUsers.UserId,
                UserName = TestUsers.UserUserName,
                FirstName = TestUsers.UserUserName,
                LastName = TestUsers.UserUserName,
                Email = TestUsers.UserEmail
            };

            var results = new[]
            {
                await userManager.CreateAsync(admin, TestUsers.Password),
                await userManager.AddToRolesAsync(admin, new string[] { RoleNames.Administrator, RoleNames.User }),

                await userManager.CreateAsync(hotelAdmin, TestUsers.Password),
                await userManager.AddToRoleAsync(hotelAdmin, RoleNames.User),

                await userManager.CreateAsync(user, TestUsers.Password),
                await userManager.AddToRoleAsync(user, RoleNames.User),
            };

            if (results.Any(r => !r.Succeeded))
            {
                var errors = results
                    .SelectMany(r => r.Errors)
                    .Select(e => e.Description);

                throw new InvalidOperationException(
                    $"Failed to seed users: {string.Join(", ", errors)}");
            }


        }

        private static async Task SeedCountriesAndHotelsAsync(
            HotelListingDbContext context)
        {

            await context.Database.OpenConnectionAsync();
            try
            {

                await context.Database.ExecuteSqlRawAsync(
                "SET IDENTITY_INSERT Countries ON");

                await context.Countries.AddRangeAsync(
                    new Country
                    {
                        CountryId = TestCountries.CountryOneId,
                        Name = TestCountries.CountryOneName,
                        ShortName = TestCountries.CountryOneShortName
                    },
                    new Country
                    {
                        CountryId = TestCountries.CountryTwoId,
                        Name = TestCountries.CountryTwoName,
                        ShortName = TestCountries.CountryTwoShortName
                    });

                await context.SaveChangesAsync();

                await context.Database.ExecuteSqlRawAsync(
                    "SET IDENTITY_INSERT Countries OFF");

                await context.Database.ExecuteSqlRawAsync(
                    "SET IDENTITY_INSERT Hotels ON");

                await context.Hotels.AddAsync(
                    new Hotel
                    {
                        Id = TestHotels.HotelOneId,
                        Name = TestHotels.HotelOneName,
                        Address = TestHotels.HotelOneAddress,
                        Rating = TestHotels.HotelOneRating,
                        NightlyRate = TestHotels.HotelOneNightlyRate,
                        CountryId = TestHotels.HotelOneCountryId
                    });

                await context.SaveChangesAsync();

                await context.Database.ExecuteSqlRawAsync(
                    "SET IDENTITY_INSERT Hotels OFF");
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }

        }

    }
}