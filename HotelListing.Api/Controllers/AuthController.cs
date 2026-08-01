using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]


    public class AuthController(IUserService userService) : ApiControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {

            var result = await userService.RegisterUserAsync(registerUserDto);

            return ToActionResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);

            if(user == null)
            {
                return Unauthorized(new { message = "Invalid Credentials" });

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

