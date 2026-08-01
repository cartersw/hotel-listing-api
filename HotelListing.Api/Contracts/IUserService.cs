using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts
{
    public interface IUserService
    {
        Task<Result> LoginAsync(LoginUserDto loginUserDto);
        Task<Result> RegisterAsync(RegisterUserDto registerUserDto);

    }
}