using workOut.Data;
using workOut.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using workOut.DTOs;

namespace workOut.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkoutController : ControllerBase
    {
        private readonly AppDbContext _context;
        // private readonly UserManager<User> _userManager;

        public WorkoutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DaysWorkout>>> GetAllWorkoutsByDay()
        {
            var userId = int.Parse(User.FindFirst(a => a.Type == "id")?.Value);
            var workouts = await _context.Days
                .Where(d => d.Workout!.UserId == userId)
                .Select(d => new DaysWorkout
                {
                    Id = d.Workout!.Id,
                    Title = d.Workout!.Title,
                    Description = d.Workout!.Description,
                    Date = d.Workout!.Date,
                    Duration = d.Workout!.Duration,
                    Type = d.Workout!.Type,
                    Day = new DayDto
                    {
                        Id = d.Id,
                        DayOfWeek = d.DayOfWeek.ToString(),
                        IsDone = d.IsDone,
                    },
                })
                .ToListAsync();
            return Ok(workouts);
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<WorkoutDto>>> GetAllWorkouts()
        // {
        //     var userId = int.Parse(User.FindFirst(a => a.Type == "id")?.Value);
        //     var workouts = await _context.Workouts
        //         .Where(w => w.UserId == userId)
        //         .Include(w => w.Days)
        //         .Select(w => new WorkoutDto
        //         {
        //             Id = w.Id,
        //             Title = w.Title,
        //             Description = w.Description,
        //             Date = w.Date,
        //             Duration = w.Duration,
        //             Type = w.Type,
        //             Days = w.Days.Select(d => new DayDto
        //             {
        //                 Id = d.Id,
        //                 DayOfWeek = d.DayOfWeek,
        //                 IsDone = d.IsDone,
        //             }).ToList(),
        //             CreatedAt = w.CreatedAt,
        //             UpdatedAt = w.UpdatedAt
        //         })
        //         .OrderByDescending(w => w.Date)
        //         .ToListAsync();
        //     return Ok(workouts);
        // }

        [HttpGet("{id}")]
        public async Task<ActionResult<Workout>> GetWorkoutById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound(new { message = "Workout not found." });

            return Ok(workout);
        }

        [HttpPost]
        public async Task<ActionResult<Workout>> CreateWorkout([FromBody] CreateWorkoutDto workoutDto)
        {
            
            var userId = int.Parse(User.FindFirst(a => a.Type == "id")?.Value);
            
            var workout = new Workout
            {
                Title = workoutDto.Title,
                Description = workoutDto.Description,
                Date = workoutDto.Date,
                Duration = workoutDto.Duration,
                Type = (WorkoutType)workoutDto.Type,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Days = workoutDto.Days.Select(d => new Day
                {
                    DayOfWeek = (DayOfWeek)d,
                    IsDone = false,
                }).ToList()
            };

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWorkoutById), new { id = workout.Id }, workout);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkout(int id, [FromBody] UpdateWorkoutDto workoutDto)
        {
            var userId = int.Parse(User.FindFirst(a => a.Type == "id")?.Value);

            if (id != userId)
                return BadRequest(new { message = "ID mismatch." });

            var existingWorkout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (existingWorkout == null)
                return NotFound(new { message = "Workout not found." });

            existingWorkout.Title = workoutDto.Title;
            existingWorkout.Description = workoutDto.Description;
            existingWorkout.Date = workoutDto.Date;
            existingWorkout.Duration = workoutDto.Duration;
            existingWorkout.Type = workoutDto.Type;
            existingWorkout.Days = workoutDto.Days;
            existingWorkout.UpdatedAt = DateTime.UtcNow;

            _context.Workouts.Update(existingWorkout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var userId = int.Parse(User.FindFirst(a => a.Type == "id")?.Value);
            var workout = await _context.Workouts
                .Include(w => w.Days)
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound(new { message = "Workout not found or you don't have permission to delete it." });

            _context.Days.RemoveRange(workout.Days);
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("types")]
        public IActionResult GetWorkoutTypes()
        {
            var types = Enum.GetValues(typeof(WorkoutType))
                .Cast<WorkoutType>()
                .Select(t => new { Id = (int)t, Name = t.ToString() })
                .ToList();

            return Ok(types);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetWorkoutStats()
        {
            var userId = int.Parse(User.FindFirst(a => a.Type == "id")?.Value);
            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .ToListAsync();

            var stats = new
            {
                TotalWorkouts = workouts.Count,
                TotalDuration = workouts.Sum(w => w.Duration),
                AverageDuration = workouts.Any() ? workouts.Average(w => w.Duration) : 0,
                WorkoutsByType = workouts.GroupBy(w => w.Type)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToList(),
                CompletedWorkouts = workouts.Count(w => w.IsDone),
                PendingWorkouts = workouts.Count(w => !w.IsDone)
            };

            return Ok(stats);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateWorkoutStatus(int id, [FromBody] bool isDone)
        {
            var userId = int.Parse(User.FindFirst(a => a.Type == "id")?.Value);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound(new { message = "Workout not found." });

            workout.IsDone = isDone;
            workout.UpdatedAt = DateTime.UtcNow;

            _context.Workouts.Update(workout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("day/{dayId}")]
        public async Task<IActionResult> UpdateWorkoutDayStatus(int dayId, [FromBody] UpdateWorkoutStatusDto dayDto)
        {
            var userId = int.Parse(User.FindFirst(a => a.Type == "id")?.Value);
            var day = await _context.Days    
                .FirstOrDefaultAsync(d => d.Id == dayId && d.Workout!.UserId == userId);

            if (day == null)
                return NotFound(new { message = "Workout day not found." });

            day.IsDone = dayDto.IsDone;

            _context.Days.Update(day);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}