using HotelListing.Api.DTOs.Hotel;
namespace HotelListing.Api.DTOs.Country
{
    public record GetCountryDetailsDto(
        int CountryId,
        string Name,
        string ShortName,
        List<GetHotelDto> Hotels
    );



}
