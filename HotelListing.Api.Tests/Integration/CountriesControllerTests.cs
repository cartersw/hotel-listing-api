using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Tests.Auth;
using HotelListing.Api.Tests.Integration.Config;
using HotelListing.Api.Tests.Models;
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


    }
}
