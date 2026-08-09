using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts
{
    public interface IUserService
    {
        string UserId { get; }
        Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<Result> AssignRoleAsync(Guid userId, AssignRoleDto assignRoleDto);
        Task<Result<LoggedInUserDto>> LoginUserAsync(LoginUserDto loginUserDto);
    }
}