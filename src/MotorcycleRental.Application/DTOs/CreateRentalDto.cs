using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class CreateRentalDto
    {
        [JsonPropertyName("entregador_id")]
        public string DeliveryDriverId { get; init; } = string.Empty;

        [JsonPropertyName("moto_id")]
        public string MotorcycleId { get; init; } = string.Empty;

        [JsonPropertyName("data_inicio")]
        public DateTime StartDate { get; init; }

        [JsonPropertyName("data_termino")]
        public DateTime EndDate { get; init; }

        [JsonPropertyName("data_previsao_termino")]
        public DateTime ExpectedEndDate { get; init; }

        [JsonPropertyName("plano")]
        public int Plan { get; init; }
    }
}
