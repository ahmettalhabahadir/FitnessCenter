using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class TrainerService
    {
        [Required]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}

