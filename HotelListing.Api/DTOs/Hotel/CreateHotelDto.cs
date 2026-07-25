using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Hotel
{
    public class CreateHotelDto
    {
        [Required]
        public string Name { get; set; }

        [MaxLength(150)]
        public string Address { get; set; }

        [Range(1.0, 5.0)]
        public double Rating { get; set; }

        [Required]
        public int CountryId { get; set; }
    }

}

