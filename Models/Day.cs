using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace workOut.Models
{
    public class Day
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        public int WorkoutId { get; set; }
        [JsonIgnore]
        public Workout? Workout { get; set; }

        public bool IsDone { get; set; }
    }
}