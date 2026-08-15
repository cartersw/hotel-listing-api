using System.ComponentModel.DataAnnotations;
namespace HotelListing.Api.Application.DTOs.Country
{
    public class UpdateCountryDto
    {
        [Required]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(4)]
        public string ShortName { get; set; }
    }




   
}
