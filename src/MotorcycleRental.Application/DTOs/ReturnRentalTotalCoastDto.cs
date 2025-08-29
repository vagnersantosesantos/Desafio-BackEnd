using System.Text.Json.Serialization;

namespace MotorcycleRental.Application.DTOs
{
    public class ReturnRentalTotalCoastDto
    {
        public ReturnRentalTotalCoastDto(decimal totalCost)
        {
            TotalCost = totalCost;
        }

        [JsonPropertyName("valor_total")]
        public decimal TotalCost { get; init; }
    }
}
