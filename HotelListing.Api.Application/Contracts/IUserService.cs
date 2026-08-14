using HotelListing.Api.Application.DTOs.Auth;
using HotelListing.Api.Common.Results;

namespace HotelListing.Api.Application.Contracts
{
    public interface IUserService
    {
        string UserId { get; }
        Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<Result> AssignRoleAsync(Guid userId, AssignRoleDto assignRoleDto);
        Task<Result<LoggedInUserDto>> LoginUserAsync(LoginUserDto loginUserDto);
    }
}