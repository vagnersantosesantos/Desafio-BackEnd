namespace MotorcycleRental.Application.DTOs
{
    public class MotorcycleDto
    {
        public MotorcycleDto(Guid id, int year, string model, string licensePlate, DateTime createdAt)
        {
            Id = id;
            Year = year;
            Model = model;
            LicensePlate = licensePlate;
            CreatedAt = createdAt;
        }

        public Guid Id { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
