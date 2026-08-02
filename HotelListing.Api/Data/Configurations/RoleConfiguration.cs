using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Api.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "ef3013de-7dad-4310-813b-d7d4486874db",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    ConcurrencyStamp = "ef3013de-7dad-4310-813b-d7d4486874db"
                },
                new IdentityRole
                {
                    Id = "8d139698-a775-4684-abf8-2765e9bd24ce",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "8d139698-a775-4684-abf8-2765e9bd24ce"
                }
                );
        }
    }
}
