using System.ComponentModel.DataAnnotations.Schema;

namespace FollowUp.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }

        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        [NotMapped]
        public TimeOnly fromdate
        {
            get => TimeOnly.FromTimeSpan(FromTime);
            set => FromTime = value.ToTimeSpan();
        }

        [NotMapped]
        public TimeOnly to
        {
            get => TimeOnly.FromTimeSpan(ToTime);
            set => ToTime = value.ToTimeSpan();
        }

        public int Status { get; set; }

        [ForeignKey("TrainerId")]
        public string TrainerId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
