using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenter.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string IdentityUserId { get; set; } = string.Empty;
        
        [ForeignKey("IdentityUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [Range(50, 250)]
        public decimal? Height { get; set; } // in cm

        [Range(30, 300)]
        public decimal? Weight { get; set; } // in kg

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(500)]
        public string? Goals { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
