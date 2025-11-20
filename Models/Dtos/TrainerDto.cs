namespace FitnessCenter.Models.Dtos
{
    public class TrainerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
        public string? Specializations { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}

