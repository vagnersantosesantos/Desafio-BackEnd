namespace MotorcycleRental.Application.DTOs
{
    public class MotorcycleRegisteredEventDto
    {
        private DateTime utcNow;

        public MotorcycleRegisteredEventDto(string id, int year, string model, string licensePlate, DateTime utcNow)
        {
            Id = id;
            Year = year;
            Model = model;
            LicensePlate = licensePlate;
            this.utcNow = utcNow;
        }

        public string Id { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
