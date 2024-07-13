using FollowUp.Models;

namespace FollowUp.ViewModels
{
    public class SuperAdminHomeVM
    {
        public IEnumerable<ApplicationUser>? Trainer { get; set; }
        public IEnumerable<ApplicationUser>? Trainee { get; set; }
    }
}