using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string UserFullName { get; set; } = null!; // اسم المدرب

        [ForeignKey("DepartmentId")]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }  

    }
}
