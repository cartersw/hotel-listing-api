namespace HotelListing.Api.DTOs.Booking
{
    public record GetBookingDto(
        int Id,
        int HotelId,
        string HotelName,
        DateOnly Checkin,
        DateOnly Checkout,
        int Guests,
        decimal TotalPrice,
        string Status,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc
        );
     
}