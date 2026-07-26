using HotelListing.Api.DTOs.Country;

namespace HotelListing.Api.Contracts
{
    public interface ICountriesService
    {
        Task<bool> CountryExistsAsync(int? countryId);
        Task<GetCountryDto> CreateCountryAsync(CreateCountryDto countryDto);
        Task DeleteCountryAsync(int? countryId);
        Task<IEnumerable<GetCountryDto>> GetCountriesAsync();
        Task<GetCountryDetailsDto?> GetCountryAsync(int countryId);
        Task UpdateCountryAsync(int? countryId, UpdateCountryDto countryDto);
    }
}