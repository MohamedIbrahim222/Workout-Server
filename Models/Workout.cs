using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace workOut.Models
{
    public class Workout
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int Duration { get; set; } // Duration in minutes

        [Required]
        public WorkoutType Type { get; set; }

        public ICollection<Day> Days { get; set; } = new List<Day>();

        public bool IsDone { get; set; }

        public int? UserId { get; set; } // Foreign key
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum WorkoutType
    {
        Cardio,
        Strength,
        Flexibility,
        Balance,
        Other
    }
}