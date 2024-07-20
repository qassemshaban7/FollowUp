using System.ComponentModel.DataAnnotations;

namespace FollowUp.ViewModels
{
    public class EditSupervisorVM
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
    }
}
