using System.ComponentModel.DataAnnotations;
namespace HotelListing.Api.DTOs.Country
{
    public class CreateCountryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(4)]
        public string ShortName { get; set; }
    }



}
