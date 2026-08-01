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
        public async Task<Result> RegisterAsync(RegisterUserDto registerUserDto)
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
                return Result.BadRequest(errors);
            }

            return Result.Success();
        }

        [HttpPost("login")]
        public async Task<Result> LoginAsync(LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);

            if (user == null)
            {
                return Result.Unauthorized();

            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid Credentials" });
            }

            return Ok();
        }
    }
}
