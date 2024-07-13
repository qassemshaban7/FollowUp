using Microsoft.AspNetCore.Identity;

namespace FollowUp.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string ArabicRoleName { get; set; }
    }
}
