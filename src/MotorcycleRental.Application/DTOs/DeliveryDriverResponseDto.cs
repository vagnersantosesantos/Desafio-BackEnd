using Domain.Enum;
using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class DeliveryDriverResponseDto
    {
        [JsonPropertyName("identificador")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("nome")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("cnpj")]
        public string CNPJ { get; init; } = string.Empty;

        [JsonPropertyName("data_nascimento")]
        public DateTime BirthDate { get; init; }

        [JsonPropertyName("numero_cnh")]
        public string LicenseNumber { get; init; } = string.Empty;

        [JsonPropertyName("tipo_cnh")]
        public LicenseType LicenseType { get; init; }

        [JsonPropertyName("imagem_cnh")]
        public string? LicenseImagePath { get; init; }
    }
}
