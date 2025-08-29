using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class ErrorResponseDto
    {
        [JsonPropertyName("mensagem")]
        public string Message { get; init; } = string.Empty;
    }
}
