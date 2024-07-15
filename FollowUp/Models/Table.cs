using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FollowUp.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int ContactHours { get; set; }
        public int AccountingHours { get; set; }
        public string TypeDivition { get; set; }
        public string Day { get; set; }
        [NotMapped]
        public TimeOnly Time { get; set; }  
        public int Capacity { get; set; }
        public int Registered { get; set; }
        public double Stay { get; set; }


        [ForeignKey("DepartmentId")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [ForeignKey("CourseId")]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [ForeignKey("TrainerId")]
        public string TrainerId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("BuildId")]
        public int BuildId { get; set; }
        public Build? Build { get; set; }
    }
}
