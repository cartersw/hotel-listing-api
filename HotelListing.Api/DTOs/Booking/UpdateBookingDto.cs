namespace HotelListing.Api.DTOs.Booking
{
    public class UpdateBookingDto
    {
        DateOnly Checkin { get; set; }
        DateOnly CheckOut { get; set; }
        int Guests { get; set; }
        
    }
}
