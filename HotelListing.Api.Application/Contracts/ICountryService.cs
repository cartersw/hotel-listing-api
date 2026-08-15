using HotelListing.Api.Application.DTOs.Country;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Common.Models.Filtering;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using Microsoft.AspNetCore.JsonPatch;

namespace HotelListing.Api.Application.Contracts
{
    public interface ICountryService
    {
        Task<bool> CountryExistsAsync(int? countryId);
        Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto);
        Task<Result> DeleteCountryAsync(int? countryId);
        Task<Result<IEnumerable<GetCountryDto>>> GetCountriesAsync(CountryFilterParameters countryFilterParameters);
        Task<Result<GetCountryDetailsDto>> GetCountryAsync(int countryId);
        Task<Result<PagedResult<GetHotelDetailsDto>>> GetCountryHotelsAsync(int countryId, PaginationParameters paginationParameters);
        Task<Result> UpdateCountryAsync(int countryId, UpdateCountryDto countryDto);
        Task<Result> PatchCountryAsync(int id, JsonPatchDocument<UpdateCountryDto> patchDoc);
    }
}