
using workOut.DTOs;
using workOut.Models;

public class DaysWorkout
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public int Duration { get; set; }
    public WorkoutType Type { get; set; }
    public DayDto Day { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsDone { get; set; }
}
