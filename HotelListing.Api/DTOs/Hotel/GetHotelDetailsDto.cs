namespace HotelListing.Api.DTOs.Hotel
{
    public record GetHotelDetailsDto(
        int Id,
        string Name,
        string Address,
        double Rating,
        string Country
    );

}

