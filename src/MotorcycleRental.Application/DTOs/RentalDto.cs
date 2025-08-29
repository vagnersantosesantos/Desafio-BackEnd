using Domain.Enum;

namespace MotorcycleRental.Application.DTOs
{
    public class RentalDto
    {
        public RentalDto(string id, string motorcycleId, string deliveryDriverId, DateTime startDate, DateTime endDate, DateTime expectedEndDate, DateTime? actualEndDate, RentalPlan plan, decimal dailyRate, decimal? totalCost)
        {
            Id = id;
            DailyRate = dailyRate;
            DeliveryDriverId = deliveryDriverId;
            MotorcycleId = motorcycleId;
            StartDate = startDate;
            EndDate = endDate;
            ExpectedEndDate = expectedEndDate;
            ActualEndDate = actualEndDate;
        }

        public string Id { get; set; }
        public string MotorcycleId { get; set; }
        public string DeliveryDriverId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public RentalPlan Plan { get; set; }
        public decimal DailyRate { get; set; }
        public decimal? TotalCost { get; set; }
        public RentalPlan Plan1 { get; }
    }
}
