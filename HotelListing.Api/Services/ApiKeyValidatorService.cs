using HotelListing.Api.Contracts;

namespace HotelListing.Api.Services
{
    public class ApiKeyValidatorService : IApiKeyValidatorService
    {
        public Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
