using HotelListing.Api.Common.Constants;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelListing.Api.Common.Results;
using Microsoft.Extensions.Options;
using HotelListing.Api.Domain;
using HotelListing.Api.Application.DTOs.Auth;
using HotelListing.Api.Application.Contracts;
using Microsoft.AspNetCore.Http;
using HotelListing.Api.Common.Models.Config;

namespace HotelListing.Api.Application.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IOptions<JwtSettings> jwtOptions,
        IHttpContextAccessor httpContextAccessor) : IUserService
    {

        public string UserId => httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(JwtRegisteredClaimNames.Sub)?
            .Value 
            ??  string.Empty;

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

            await userManager.AddToRoleAsync(user, RoleNames.User);

            return Result<RegisteredUserDto>.Success(registeredUser);
        }

        
        public async Task<Result<LoggedInUserDto>> LoginUserAsync(LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);

            if (user == null)
            {
                return Result<LoggedInUserDto>.Unauthorized(new Error(ErrorCodes.Unauthorized, "Invalid credentials"));

            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!isPasswordValid)
            {
                return Result<LoggedInUserDto>.Unauthorized(new Error(ErrorCodes.Unauthorized, "Invalid credentials"));
            }

            var token = await GenerateToken(user);

            var loggedInUserDto = new LoggedInUserDto { 
                Token  = token
            };


            return Result<LoggedInUserDto>.Success(loggedInUserDto);
        }

        public async Task<Result> AssignRoleAsync(Guid userId, AssignRoleDto assignRoleDto)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if(user == null)
            {
                return Result.NotFound();
            }

            if(!await roleManager.RoleExistsAsync(assignRoleDto.RoleName))
            {
                return Result.BadRequest(new Error(ErrorCodes.BadRequest, "Role does not exist"));
            }

            if(await userManager.IsInRoleAsync(user, assignRoleDto.RoleName))
            {
                return Result.Failure(new Error(ErrorCodes.Conflict, "User already has this role"));
            }

            await userManager.AddToRoleAsync(user, assignRoleDto.RoleName);

            return Result.Success();
        }




        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Id),
                new (JwtRegisteredClaimNames.Email, user.Email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Name, user.FullName),

            };

            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            claims = claims.Union(roleClaims).ToList();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Value.Issuer,
                audience: jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtOptions.Value.DurationInMinutes)),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token); 

        }
    }
}
