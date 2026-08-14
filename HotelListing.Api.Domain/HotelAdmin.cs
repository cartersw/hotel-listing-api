namespace HotelListing.Api.Domain
{
    public class HotelAdmin
    {
        public int Id { get; set; }

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }
    }
}
