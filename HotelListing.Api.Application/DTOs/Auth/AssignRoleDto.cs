using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Application.DTOs.Auth
{
    public class AssignRoleDto
    {
        [Required]
        public string RoleName { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
