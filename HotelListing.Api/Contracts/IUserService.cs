using HotelListing.Api.Common.Results;
using HotelListing.Api.DTOs.Auth;

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