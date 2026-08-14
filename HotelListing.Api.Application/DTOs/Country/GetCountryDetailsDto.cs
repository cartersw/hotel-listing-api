using HotelListing.Api.Application.DTOs.Hotel;

namespace HotelListing.Api.Application.DTOs.Country
{
    public record GetCountryDetailsDto(
        int CountryId,
        string Name,
        string ShortName,
        List<GetHotelDto> Hotels
    );



}
