using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HotelListing.Api.Application.DTOs.Auth;
using HotelListing.Api.Application.Contracts;

namespace HotelListing.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    


    public class AuthController(IUserService userService) : ApiControllerBase
    {

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {

            var result = await userService.RegisterUserAsync(registerUserDto);

            return ToActionResult(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoggedInUserDto>> Login(LoginUserDto loginUserDto)
        {
            var result = await userService.LoginUserAsync(loginUserDto);

            return ToActionResult(result);
        }

        [HttpPost("{userId:guid}/roles")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> AssignRole(Guid userId, AssignRoleDto assignRoleDto)
        {
            var result = await userService.AssignRoleAsync(userId, assignRoleDto);

            return ToActionResult(result);
        }

    }

}

