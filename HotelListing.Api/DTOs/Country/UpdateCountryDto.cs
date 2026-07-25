using System.ComponentModel.DataAnnotations;
namespace HotelListing.Api.DTOs.Country
{
    public class UpdateCountryDto : CreateCountry
    {
        [Required]
        public int CountryId { get; set; }
    }




   
}
