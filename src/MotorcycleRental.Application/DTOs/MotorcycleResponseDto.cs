using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class MotorcycleResponseDto
    {
        [JsonPropertyName("identificador")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("ano")]
        public int Year { get; init; }

        [JsonPropertyName("modelo")]
        public string Model { get; init; } = string.Empty;

        [JsonPropertyName("placa")]
        public string LicensePlate { get; init; } = string.Empty;
    }
}
