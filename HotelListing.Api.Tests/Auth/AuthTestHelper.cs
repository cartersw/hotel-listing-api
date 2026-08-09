using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;
using HotelListing.Api.Tests.DTOs;
using System.Net.Http.Headers;


namespace HotelListing.Api.Tests.Auth
{
    internal static class AuthTestHelper
    {
        public static async Task<string> LoginAsync(
            HttpClient client,
            string email,
            string password)
        {
            var response = await client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    email,
                    password
                });

            response.EnsureSuccessStatusCode();

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return loginResponse!.Token;
        }

        public static async Task AuthenticateAsync(HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
