using HotelListing.Api.Authorization.Requirements;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelListing.Api.Authorization.Handlers
{
    public class ManageHotelAuthorizationHandler(HotelListingDbContext dbContext) 
        : AuthorizationHandler<ManageHotelRequirement, int>
    {
      
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext authContext, 
            ManageHotelRequirement requirement, 
            int hotelId)
        {
            if (authContext.User.IsInRole(RoleNames.Administrator))
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
            ha.UserId == userId && ha.HotelId == hotelId);

            if (managesHotel)
            {
                authContext.Succeed(requirement);
            }


        }
    }
}
