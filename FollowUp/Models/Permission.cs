using System.ComponentModel.DataAnnotations.Schema;

namespace FollowUp.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }

        [NotMapped]
        public TimeOnly fromdate { get; set; }
        [NotMapped]
        public TimeOnly to { get; set; }


        [ForeignKey("TrainerId")]
        public string TrainerId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
