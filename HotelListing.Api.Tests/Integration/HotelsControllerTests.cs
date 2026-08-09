using HotelListing.Api.Tests.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HotelListing.Api.Tests.Integration
{
    public class HotelsControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetHotels_WhenAuthenticated_ReturnsOk()
        {
            var token = await AuthTestHelper.LoginAsync(_client, "basictestuser@test.com", "TestPassword1!");

            await AuthTestHelper.AuthenticateAsync(_client, token);

            var response = await _client.GetAsync("/api/hotels");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
