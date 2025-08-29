using Domain.Enum;

namespace Domain.Entities
{
    public class Rental
    {
        public string Id { get; set; } = string.Empty;
        public string MotorcycleId { get; set; } = string.Empty;
        public string DeliveryDriverId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public RentalPlan Plan { get; set; }
        public decimal DailyRate { get; set; }
        public decimal? TotalCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Motorcycle Motorcycle { get; set; } = null!;
        public DeliveryDriver DeliveryDriver { get; set; } = null!;
    }
}
