using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Hotel
{
    public class AddHotelAdminDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
