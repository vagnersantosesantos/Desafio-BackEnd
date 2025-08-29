using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class CreateMotorcycleDto
    {
        public CreateMotorcycleDto(string id, int year, string model, string licensePlate)
        {
            Id = id;
            Year = year;
            Model = model;
            LicensePlate = licensePlate;
        }

        [JsonPropertyName("identificador")]
        public string Id { get; set; }

        [JsonPropertyName("ano")]
        public int Year { get; set; }

        [JsonPropertyName("modelo")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = string.Empty;
    }
}
