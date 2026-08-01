using HotelListing.Api.Constants;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HotelListing.Api.Contracts;

namespace HotelListing.Api.Services
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        public async Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var user = new ApplicationUser
            {
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                UserName = registerUserDto.Email
            };

            var result = await userManager.CreateAsync(user, registerUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
                return Result<RegisteredUserDto>.BadRequest(errors);
            }

            var registeredUser = new RegisteredUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return Result<RegisteredUserDto>.Success(registeredUser);
        }

        [HttpPost("login")]
        public async Task<Result<string>> LoginUserAsync(LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);

            if (user == null)
            {
                return Result<string>.Unauthorized(new Error(ErrorCodes.Unauthorized, "Invalid credentials"));

            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!isPasswordValid)
            {
                return Result<string>.Unauthorized(new Error(ErrorCodes.Unauthorized, "Invalid credentials"));
            }

            return Result<string>.Success("Login succesful");
        }
    }
}
