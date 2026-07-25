namespace HotelListing.Api.DTOs.Hotel
{
    public record GetHotelsDto(
        int TotalCount, 
        List<GetHotelDto> Hotels
    );

}

