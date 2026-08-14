namespace HotelListing.Api.Application.DTOs.Country
{
    public record GetCountriesDto(
        int TotalCount,
        List<GetCountryDto> Countries
    );




   
}
