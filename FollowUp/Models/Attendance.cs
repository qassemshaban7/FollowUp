using System.ComponentModel.DataAnnotations.Schema;

namespace FollowUp.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int? Minutes { get; set; }
        public DateTime Date { get; set; }
        public string HijriDate { get; set; }


        public int Status { get; set; }  // 1-Not respond yet  2- to Head of Dept   3- to Admin  4-Finsh in System  5- out OF System 
        public int? Note { get; set; } // 2- Done

        public string? Statment { get; set; }
        public string? StatmentDate { get; set; }


        public string? FirstAnswer { get; set; }
        public string? HeadOfDeptName { get; set; }   
        public string? HeadOfDeptSignture { get; set; }   
        public string? HeadOfDeptSendDate { get; set; }


        public string? SecondAnswer { get; set; }
        public string?  DeanStatment { get; set; }
        public string?  DeanName { get; set; }


        [ForeignKey("TrainerId")]
        public string TrainerId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("TableId")]
        public int TableId { get; set; }
        public Table? Table { get; set; }
    }
}
