using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models.ViewModels
{
    public class CreateAppointmentViewModel
    {
        [Required(ErrorMessage = "Lütfen bir spor salonu seçin")]
        [Display(Name = "Spor Salonu")]
        public int GymId { get; set; }

        [Required(ErrorMessage = "Lütfen bir hizmet seçin")]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Lütfen bir antrenör seçin")]
        [Display(Name = "Antrenör")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Lütfen bir tarih seçin")]
        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Lütfen başlangıç saati seçin")]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [StringLength(500)]
        [Display(Name = "Notlar (İsteğe Bağlı)")]
        public string? Notes { get; set; }
    }
}

