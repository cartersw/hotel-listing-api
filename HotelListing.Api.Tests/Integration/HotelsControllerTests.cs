using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Tests.Auth;
using HotelListing.Api.Tests.Integration.Config;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace HotelListing.Api.Tests.Integration
{
    [Collection("Integration Tests")]
    public class HotelsControllerTests(CustomWebApplicationFactory factory) 
    {
        private readonly HttpClient _client = factory.CreateClient();


        [Fact]
        public async Task CreateHotel_AsAdmin_ReturnsCreated()
        {
            var token = await AuthTestHelper.LoginAsync(_client, TestUsers.AdminEmail, TestUsers.Password);

            await AuthTestHelper.AuthenticateAsync(_client, token);

            var hotelDto = new CreateHotelDto
            {
                Name = "TestCreateHotel",
                Address = "address",
                Rating = 5,
                CountryId = TestCountries.CountryOneId
            };

            var response = await _client.PostAsJsonAsync("/api/hotels", hotelDto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetHotels_WhenAuthenticated_ReturnsOk()
        {
            var token = await AuthTestHelper.LoginAsync(_client, TestUsers.UserEmail, TestUsers.Password);

            await AuthTestHelper.AuthenticateAsync(_client, token);

            var response = await _client.GetAsync("/api/hotels");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GiveManagerOne_HotelAdminForOne_ReturnsNoContent()
        {
            var token = await AuthTestHelper.LoginAsync(_client, TestUsers.AdminEmail, TestUsers.Password);

            await AuthTestHelper.AuthenticateAsync(_client, token);

            var addAdminDto = new AddHotelAdminDto
            {
                UserId = TestUsers.ManagerOneId
            };

            var response = await _client.PostAsJsonAsync("/api/hotels/" + TestHotels.HotelOneId + "/admins", addAdminDto);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }


        [Fact]
        public async Task GetBookings_AsHotelAdminForHotel_ReturnsOk()
        {
            var token = await AuthTestHelper.LoginAsync(_client, TestUsers.ManagerOneEmail, TestUsers.Password);

            await AuthTestHelper.AuthenticateAsync(_client, token);

            var response = await _client.GetAsync("/api/hotels/" + TestHotels.HotelOneId + "/bookings");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
