using HotelListing.Api.Authorization.Requirements;
using HotelListing.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelListing.Api.Authorization.Handlers
{
    public class ManageHotelAuthorizationHandler(HotelListingDbContext dbContext) 
        : AuthorizationHandler<ManageHotelRequirement, Hotel>
    {
      
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext authContext, 
            ManageHotelRequirement requirement, 
            Hotel resource)
        {
            if (authContext.User.IsInRole("Administrator"))
            {
                authContext.Succeed(requirement);
                return;
            }

            var userId = authContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if(userId == null)
            {
                return;
            }

            var managesHotel = await dbContext.HotelAdmins.AnyAsync(ha =>
            ha.UserId == userId && ha.HotelId == resource.Id);

            if (managesHotel)
            {
                authContext.Succeed(requirement);
            }


        }
    }
}
