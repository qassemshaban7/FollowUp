using System.ComponentModel.DataAnnotations.Schema;

namespace FollowUp.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int? Minutes { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("TrainerId")]
        public string TrainerId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("TableId")]
        public int TableId { get; set; }
        public Table? Table { get; set; }
    }
}
