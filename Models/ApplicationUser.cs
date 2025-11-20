using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenter.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Admin'in yönettiği spor salonu (nullable - sadece admin için)
        public int? GymId { get; set; }
        
        [ForeignKey("GymId")]
        public Gym? Gym { get; set; }
    }
}

