using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class ReturnRentalDto
    {
        [JsonPropertyName("data_devolucao")]
        public DateTime ReturnDate { get; init; }
    }
}
