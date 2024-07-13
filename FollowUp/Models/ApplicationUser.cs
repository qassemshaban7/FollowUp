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
        public string UserFullName { get; set; } = null!; // اسم المتدرب
        public string? EnglishFullName { get; set; }

        public long? RegisterNum { get; set; } // رقم السجل
        public string? Status { get; set; } //  المرحلة
    }
}
