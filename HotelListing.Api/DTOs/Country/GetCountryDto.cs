using HotelListing.Api.DTOs.Hotel;
namespace HotelListing.Api.DTOs.Country
{
    public record GetCountryDto(
        int CountryId,
        string Name,
        string ShortName,
        List<GetHotelDto> Hotels
    );




   
}
