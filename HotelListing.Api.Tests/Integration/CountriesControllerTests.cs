using HotelListing.Api.Application.DTOs.Country;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Domain;
using HotelListing.Api.Tests.Auth;
using HotelListing.Api.Tests.Integration.Config;
using HotelListing.Api.Tests.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace HotelListing.Api.Tests.Integration
{
    [Collection("Integration Tests")]
    public class CountriesControllerTests(CustomWebApplicationFactory factory)
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]

        public async Task GetCountryHotels_ReturnsData_ReturnsOk()
        {
            var token = await AuthTestHelper.LoginAsync(_client, TestUsers.UserEmail, TestUsers.Password);

            await AuthTestHelper.AuthenticateAsync(_client, token);

            var response = await _client.GetAsync("/api/countries/" + TestCountries.CountryOneId + "/hotels");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var wrapper = await response.Content
                .ReadFromJsonAsync<PagedApiResponse<List<GetHotelDetailsDto>>>();

            Assert.NotNull(wrapper);

            Assert.NotNull(wrapper.Data);

            Assert.NotEmpty(wrapper.Data);

        }

        [Fact]

        public async Task PatchCountry_PatchesData_ReturnsNoContent()
        {
            var token = await AuthTestHelper.LoginAsync(_client, TestUsers.AdminEmail, TestUsers.Password);

            await AuthTestHelper.AuthenticateAsync(_client, token);

            var json = """
                [
                    {
                        "op": "replace",
                        "path": "/shortName",
                        "value": "BFL"
                    }
                ]

                """;

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json-patch+json");

            var response = await _client.PatchAsync("/api/countries/" + TestCountries.CountryTwoId, content);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            using var scope = factory.Services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<HotelListingDbContext>();

            var country = await context.Countries.FindAsync(TestCountries.CountryTwoId);

            Assert.NotNull(country);
            Assert.Equal("Big Florida Land", country.Name);
            Assert.Equal("BFL", country.ShortName);
            
        }


    }
}
