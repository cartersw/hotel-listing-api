namespace HotelListing.Api.Application.DTOs.Auth
{
    public record LoggedInUserDto
    {
        public string Token { get; init; } = string.Empty;
    }
}
