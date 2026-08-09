using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Booking
{
    public class CreateBookingDto
    {

        public int HotelId { get; set; }

        [Required]
        public DateOnly CheckIn { get; set; }
        [Required]
        public DateOnly CheckOut { get; set; }

        [Range(1, 10)]
        public int Guests { get; set; }
    
    }
}