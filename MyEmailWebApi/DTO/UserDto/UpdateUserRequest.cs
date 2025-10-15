using System.ComponentModel.DataAnnotations;

namespace MyEmailWebApi.DTO.UserDto
{
    public class UpdateUserRequest
    {
        // IdentityUser can hold several IdentityClaims by design
        public string[]? Departments { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        // IdentityUser can hold several IdentityRoles by design
        public bool IsAdmin { get; set; }

        public bool IsEmployee { get; set; }

        public bool IsManager { get; set; }
    }
}
