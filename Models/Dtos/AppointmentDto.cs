namespace FitnessCenter.Models.Dtos
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PriceAtBooking { get; set; }
        public string? Notes { get; set; }
    }
}

