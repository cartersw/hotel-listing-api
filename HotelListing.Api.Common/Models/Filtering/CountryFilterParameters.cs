namespace HotelListing.Api.Common.Models.Filtering
{
    public class CountryFilterParameters : BaseFilterParameters
    {
        public string? Region { get; set; }
        public bool? HasHotels { get; set; }
    }

}
