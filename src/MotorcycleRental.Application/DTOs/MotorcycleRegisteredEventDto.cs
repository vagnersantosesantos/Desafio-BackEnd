namespace MotorcycleRental.Application.DTOs
{
    public class MotorcycleRegisteredEventDto
    {
        public MotorcycleRegisteredEventDto(string id, int year, string model, string licensePlate, DateTime timestamp)
        {
            Id = id;
            Year = year;
            Model = model;
            LicensePlate = licensePlate;
            Timestamp = timestamp;
        }

        public string Id { get; }
        public int Year { get; }
        public string Model { get; }
        public string LicensePlate { get; }
        public DateTime Timestamp { get; }
    }
}
