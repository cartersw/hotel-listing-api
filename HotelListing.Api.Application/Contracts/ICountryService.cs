using HotelListing.Api.Application.DTOs.Country;
using HotelListing.Api.Common.Results;

namespace HotelListing.Api.Application.Contracts
{
    public interface ICountryService
    {
        Task<bool> CountryExistsAsync(int? countryId);
        Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto);
        Task<Result> DeleteCountryAsync(int? countryId);
        Task<Result<IEnumerable<GetCountryDto>>> GetCountriesAsync();
        Task<Result<GetCountryDetailsDto>> GetCountryAsync(int countryId);
        Task<Result> UpdateCountryAsync(int countryId, UpdateCountryDto countryDto);
    }
}