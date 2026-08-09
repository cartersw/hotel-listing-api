using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Booking
{
    public class UpdateBookingDto
    {
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }

        [Range(1, 10)]
        public int Guests { get; set; }
    }
}
