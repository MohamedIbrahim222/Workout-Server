using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace workOut.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation property for Workouts
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    }
}