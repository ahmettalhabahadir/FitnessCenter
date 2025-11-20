using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models.Dtos;

namespace FitnessCenter.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/trainers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainerDto>>> GetTrainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.Gym)
                .Select(t => new TrainerDto
                {
                    Id = t.Id,
                    FullName = $"{t.FirstName} {t.LastName}",
                    GymName = t.Gym != null ? t.Gym.Name : "N/A",
                    Specializations = t.Specializations,
                    Email = t.Email,
                    Phone = t.Phone
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: api/trainers/available?dateTime=2024-01-15T10:00:00
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<TrainerDto>>> GetAvailableTrainers([FromQuery] DateTime dateTime)
        {
            var dayOfWeek = (int)dateTime.DayOfWeek;
            var timeOfDay = dateTime.TimeOfDay;

            // Get all trainer IDs with conflicting appointments at this time
            var trainerIdsWithConflicts = await _context.Appointments
                .Where(a => a.Status != Models.AppointmentStatus.Cancelled
                    && a.StartDateTime <= dateTime.AddHours(2)
                    && a.EndDateTime > dateTime)
                .Select(a => a.TrainerId)
                .Distinct()
                .ToListAsync();

            // Get available trainers using LINQ
            var availableTrainers = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.TrainerAvailabilities)
                .Where(t => 
                    // Has availability for this day and time
                    t.TrainerAvailabilities.Any(ta => 
                        ta.DayOfWeek == dayOfWeek &&
                        TimeSpan.Parse(ta.StartTime) <= timeOfDay &&
                        TimeSpan.Parse(ta.EndTime) >= timeOfDay) &&
                    // No conflicting appointments
                    !trainerIdsWithConflicts.Contains(t.Id))
                .Select(t => new TrainerDto
                {
                    Id = t.Id,
                    FullName = $"{t.FirstName} {t.LastName}",
                    GymName = t.Gym != null ? t.Gym.Name : "N/A",
                    Specializations = t.Specializations,
                    Email = t.Email,
                    Phone = t.Phone
                })
                .ToListAsync();

            return Ok(availableTrainers);
        }
    }
}

