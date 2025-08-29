using Domain.Enum;
using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class RentalResponseDto
    {
        public RentalResponseDto(string id, decimal dailyRate, string deliveryDriverId, string motorcycleId, DateTime startDate, DateTime endDate, DateTime expectedEndDate, DateTime? actualEndDate)
        {
            Id = id;
            DailyRate = dailyRate;
            DeliveryDriverId = deliveryDriverId;
            MotorcycleId = motorcycleId;
            StartDate = startDate;
            EndDate = endDate;
            ExpectedEndDate = expectedEndDate;
            ReturnDate = actualEndDate;
        }

        [JsonPropertyName("identificador")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("valor_diaria")]
        public decimal DailyRate { get; init; }

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

        [JsonPropertyName("data_devolucao")]
        public DateTime? ReturnDate { get; init; }
    }
}
