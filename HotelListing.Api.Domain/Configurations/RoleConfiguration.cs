using HotelListing.Api.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Api.Domain.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "ef3013de-7dad-4310-813b-d7d4486874db",
                    Name = RoleNames.Administrator,
                    NormalizedName = RoleNames.Administrator.ToUpper(),
                    ConcurrencyStamp = "ef3013de-7dad-4310-813b-d7d4486874db"
                },
                new IdentityRole
                {
                    Id = "8d139698-a775-4684-abf8-2765e9bd24ce",
                    Name = RoleNames.User,
                    NormalizedName = RoleNames.User.ToUpper(),
                    ConcurrencyStamp = "8d139698-a775-4684-abf8-2765e9bd24ce"
                },
                new IdentityRole
                {
                    Id = "f823e597-8558-4810-b6e6-570c4c0fcac7",
                    Name = RoleNames.HotelAdmin,
                    NormalizedName = RoleNames.HotelAdmin.ToUpper(),
                    ConcurrencyStamp = "f823e597-8558-4810-b6e6-570c4c0fcac7"
                }
                );
        }
    }
}
