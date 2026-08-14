using HotelListing.Api.Application.Contracts;
using Microsoft.Extensions.Configuration;

namespace HotelListing.Api.Application.Services
{
    public class ApiKeyValidatorService(IConfiguration configuration) : IApiKeyValidatorService
    {
        public Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default)
        {
            return Task.FromResult(apiKey.Equals(configuration["ApiAuthentication:ApiKey"]));
        }
    }
}
