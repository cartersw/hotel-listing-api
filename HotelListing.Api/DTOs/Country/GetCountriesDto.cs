namespace HotelListing.Api.DTOs.Country
{
    public record GetCountriesDto(
        int TotalCount,
        List<GetCountryDto> Countries
    );




   
}
