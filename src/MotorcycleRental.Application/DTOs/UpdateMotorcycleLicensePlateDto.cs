using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class UpdateMotorcycleLicensePlateDto
    {
        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = string.Empty;
    }
}
