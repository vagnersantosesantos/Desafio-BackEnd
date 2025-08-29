namespace Domain.Entities
{
    public class NotificationLog
    {
        public string Id { get; set; } = string.Empty;
        public string MotorcycleId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Motorcycle Motorcycle { get; set; } = null!;
    }
}
