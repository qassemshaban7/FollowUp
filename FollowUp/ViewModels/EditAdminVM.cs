using System.ComponentModel.DataAnnotations;

namespace FollowUp.ViewModels
{
    public class EditAdminVM
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? NewPassword { get; set; }
        public string? pass { get; set; }
        public IFormFile? Image { get; set; }

        public List<string> SelectedRoles { get; set; }

        public int DepartmentId { get; set; }
    }
}
