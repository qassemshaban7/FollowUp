using FollowUp.Models;

namespace FollowUp.ViewModels
{
    public class SuperAdminHomeVM
    {
        public IEnumerable<ApplicationUser>? Trainers { get; set; }
        public IEnumerable<ApplicationUser>? Supervisors { get; set; }
    }
}