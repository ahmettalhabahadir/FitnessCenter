using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenter.Models
{
    public class TrainerAvailability
    {
        public int Id { get; set; }

        [Required]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        [Required]
        [Range(0, 6)]
        public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, ..., 6 = Saturday

        [Required]
        [StringLength(10)]
        public string StartTime { get; set; } = "09:00"; // Format: "HH:mm"

        [Required]
        [StringLength(10)]
        public string EndTime { get; set; } = "17:00"; // Format: "HH:mm"
    }
}

