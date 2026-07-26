using System.ComponentModel.DataAnnotations;
namespace HotelListing.Api.DTOs.Country
{
    public class UpdateCountryDto : CreateCountryDto
    {
        [Required]
        public int CountryId { get; set; }
    }




   
}
