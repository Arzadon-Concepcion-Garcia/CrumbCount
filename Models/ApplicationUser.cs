using Microsoft.AspNetCore.Identity;

namespace CrumbCount2.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}