using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models.Dtos;

namespace FitnessCenter.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/appointments/members/5
        [HttpGet("members/{memberId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMemberAppointments(int memberId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.Gym)
                .Where(a => a.MemberId == memberId)
                .OrderByDescending(a => a.StartDateTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ServiceName = a.Service != null ? a.Service.Name : "N/A",
                    TrainerName = a.Trainer != null ? $"{a.Trainer.FirstName} {a.Trainer.LastName}" : "N/A",
                    GymName = a.Gym != null ? a.Gym.Name : "N/A",
                    StartDateTime = a.StartDateTime,
                    EndDateTime = a.EndDateTime,
                    Status = a.Status.ToString(),
                    PriceAtBooking = a.PriceAtBooking,
                    Notes = a.Notes
                })
                .ToListAsync();

            if (!appointments.Any())
            {
                return NotFound($"No appointments found for member ID {memberId}");
            }

            return Ok(appointments);
        }
    }
}

