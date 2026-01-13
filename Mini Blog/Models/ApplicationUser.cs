using Microsoft.AspNetCore.Identity;

namespace MiniBlog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? AvatarUrl { get; set; }
    }
}
