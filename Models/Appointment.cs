using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenter.Models
{
    public enum AppointmentStatus
    {
        Pending,
        Approved,
        Cancelled
    }

    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }
        public Member? Member { get; set; }

        [Required]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        [Required]
        public int GymId { get; set; }
        public Gym? Gym { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtBooking { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

