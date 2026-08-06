using HotelListing.Api.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Data
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }

        public DateOnly CheckIn { get; set; }

        public DateOnly Checkout { get; set; }

        public int Guests { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
    }

}

