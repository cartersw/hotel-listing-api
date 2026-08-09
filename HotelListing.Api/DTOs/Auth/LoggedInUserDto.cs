namespace HotelListing.Api.DTOs.Auth
{
    public record LoggedInUserDto
    {
        public string Token { get; init; } = string.Empty;
    }
}
