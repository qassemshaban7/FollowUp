using FollowUp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace FollowUp.ViewModels
{
    public class EditTableVM
    {
        public int Id { get; set; }
        public int ContactHours { get; set; }
        public int AccountingHours { get; set; }
        public string TypeDivition { get; set; }
        public string Day { get; set; }
        public TimeOnly Time { get; set; }
        public int Capacity { get; set; }
        public int Registered { get; set; }
        public double Stay { get; set; }
        public int DepartmentId { get; set; }
        public int CourseId { get; set; }
        public string TrainerId { get; set; }
        public int BuildId { get; set; }
    }
}
