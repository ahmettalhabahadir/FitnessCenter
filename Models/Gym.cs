using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class Gym
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(20)]
        public string OpeningTime { get; set; } = "09:00";

        [Required]
        [StringLength(20)]
        public string ClosingTime { get; set; } = "22:00";

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? WorkingHours { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}

