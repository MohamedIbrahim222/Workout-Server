using System;
using System.Collections.Generic;
using workOut.Models;

namespace workOut.DTOs
{
    public class WorkoutDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public WorkoutType Type { get; set; }
        public List<DayDto> Days { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateWorkoutDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public int Type { get; set; }
        public ICollection<int> Days { get; set; }
    }

    public class UpdateWorkoutDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public WorkoutType Type { get; set; }
        public ICollection<Day> Days { get; set; }
    }

    public class UpdateWorkoutStatusDto
    {
        public bool IsDone { get; set; }
    }
}
