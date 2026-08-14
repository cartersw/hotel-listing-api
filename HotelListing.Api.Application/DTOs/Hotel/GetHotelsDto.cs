namespace HotelListing.Api.Application.DTOs.Hotel
{
    public record GetHotelsDto(
        int TotalCount, 
        List<GetHotelDto> Hotels
    );

}

