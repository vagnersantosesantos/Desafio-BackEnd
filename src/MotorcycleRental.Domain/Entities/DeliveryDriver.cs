using Domain.Enum;

namespace Domain.Entities
{
    public class DeliveryDriver
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public LicenseType LicenseType { get; set; }
        public string? LicenseImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
