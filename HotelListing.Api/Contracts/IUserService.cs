using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts
{
    public interface IUserService
    {
        Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<Result<string>> LoginUserAsync(LoginUserDto loginUserDto);
    
    }
}