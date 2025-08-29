namespace MotorcycleRental.Application.DTOs
{
    public class RentalCalculationDto
    {
        public RentalCalculationDto(decimal totalCost, decimal baseCost, decimal? penaltyCost, decimal? additionalCost, int actualDays, int planDays)
        {
            TotalCost = totalCost;
            BaseCost = baseCost;
            PenaltyCost = penaltyCost;
            AdditionalCost = additionalCost;
            ActualDays = actualDays;
            PlanDays = planDays;
        }

        public decimal TotalCost { get; set; }
        public decimal BaseCost { get; set; }
        public decimal? PenaltyCost { get; set; }
        public decimal? AdditionalCost { get; set; }
        public int ActualDays { get; set; }
        public int PlannedDays { get; set; }
        public int PlanDays { get; }
    }
}
