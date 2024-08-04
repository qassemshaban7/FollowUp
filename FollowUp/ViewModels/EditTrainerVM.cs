using System.ComponentModel.DataAnnotations;

namespace FollowUp.ViewModels
{
    public class EditTrainerVM
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public int DepartmentId { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Specialty { get; set; }
        public string? NewPassword { get; set; }

        public List<string> SelectedRoles { get; set; }

        public IFormFile? Image { get; set; }
    }
}
