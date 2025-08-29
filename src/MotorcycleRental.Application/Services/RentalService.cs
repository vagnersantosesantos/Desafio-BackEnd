using Domain.Entities;
using Domain.Enum;
using Microsoft.Extensions.Logging;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Repositories;
using MotorcycleRental.Application.Interfaces.Services;


namespace MotorcycleRental.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IDeliveryDriverRepository _driverRepository;
        private readonly ILogger<RentalService> _logger;

        private readonly Dictionary<RentalPlan, decimal> _dailyRates = new()
        {
            { RentalPlan.SevenDays, 30.00m },
            { RentalPlan.FifteenDays, 28.00m },
            { RentalPlan.ThirtyDays, 22.00m },
            { RentalPlan.FortyFiveDays, 20.00m },
            { RentalPlan.FiftyDays, 18.00m }
        };

        private readonly Dictionary<RentalPlan, decimal> _penaltyRates = new()
        {
            { RentalPlan.SevenDays, 0.20m },
            { RentalPlan.FifteenDays, 0.40m },
            { RentalPlan.ThirtyDays, 0.00m },
            { RentalPlan.FortyFiveDays, 0.00m },
            { RentalPlan.FiftyDays, 0.00m }
        };

        public RentalService(
            IRentalRepository rentalRepository,
            IMotorcycleRepository motorcycleRepository,
            IDeliveryDriverRepository driverRepository,
            ILogger<RentalService> logger)
        {
            _rentalRepository = rentalRepository;
            _motorcycleRepository = motorcycleRepository;
            _driverRepository = driverRepository;
            _logger = logger;
        }

        public async Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto dto)
        {
            _logger.LogInformation("Creating new rental for driver ID: {DriverId}, motorcycle ID: {MotorcycleId}",
                dto.DeliveryDriverId, dto.MotorcycleId);

            // Validate motorcycle exists and is available
            var motorcycle = await _motorcycleRepository.GetByIdAsync(dto.MotorcycleId);
            if (motorcycle == null)
                throw new KeyNotFoundException($"Motorcycle with ID '{dto.MotorcycleId}' not found.");

            var activeRentals = await _rentalRepository.GetActiveRentalsByMotorcycleIdAsync(dto.MotorcycleId);
            if (activeRentals.Any())
                throw new InvalidOperationException("Motorcycle is currently rented.");

            // Validate delivery driver
            var driver = await _driverRepository.GetByIdAsync(dto.DeliveryDriverId);
            if (driver == null)
                throw new KeyNotFoundException($"Delivery driver with ID '{dto.DeliveryDriverId}' not found.");

            if (driver.LicenseType != LicenseType.A && driver.LicenseType != LicenseType.AB)
                throw new InvalidOperationException("Only drivers with license type A or A+B can rent motorcycles.");

            // Create rental
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var planDays = dto.Plan;
            var rentalPlan = (RentalPlan)planDays; 
            var endDate = startDate.AddDays(planDays - 1);
            var dailyRate = _dailyRates[rentalPlan];

            var rental = new Rental
            {
                Id = Guid.NewGuid().ToString(),
                MotorcycleId = dto.MotorcycleId,
                DeliveryDriverId = dto.DeliveryDriverId,
                StartDate = startDate,
                EndDate = endDate,
                ExpectedEndDate = endDate,
                Plan = rentalPlan,
                DailyRate = dailyRate
            };

            await _rentalRepository.AddAsync(rental);

            _logger.LogInformation("Rental created successfully with ID: {RentalId}", rental.Id);

            return MapToDto(rental);
        }
        public async Task<RentalResponseDto> GetByIdAsync(string rentalId)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental == null)
                throw new KeyNotFoundException($"Rental with ID '{rentalId}' not found.");

            return MapToDto(rental);
        }
        public async Task<ReturnRentalTotalCoastDto> ReturnRentalAsync(string rentalId, DateTime returnDate)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental == null)
                throw new KeyNotFoundException($"Rental with ID '{rentalId}' not found.");

            if (rental.ActualEndDate.HasValue)
                throw new InvalidOperationException("Rental has already been completed.");

            var calculation = CalculateRentalCost(rental, returnDate);

            _logger.LogInformation("Rental cost calculated for ID: {RentalId}, Total: {TotalCost}",
                rentalId, calculation.TotalCost);

            return calculation;
        }
        private ReturnRentalTotalCoastDto CalculateRentalCost(Rental rental, DateTime returnDate)
        {
            var planDays = (int)rental.Plan;
            var actualDays = (int)(returnDate.Date - rental.StartDate.Date).TotalDays + 1;

            var baseCost = rental.DailyRate * planDays;
            decimal? penaltyCost = null;
            decimal? additionalCost = null;

            if (actualDays < planDays)
            {
                // Early return - penalty
                var unusedDays = planDays - actualDays;
                var penaltyRate = _penaltyRates[rental.Plan];
                penaltyCost = rental.DailyRate * unusedDays * penaltyRate;
            }
            else if (actualDays > planDays)
            {
                // Late return - additional cost
                var extraDays = actualDays - planDays;
                additionalCost = 50.00m * extraDays;
            }

            var totalCost = baseCost + (penaltyCost ?? 0) + (additionalCost ?? 0);

            return new ReturnRentalTotalCoastDto(totalCost);
        }
        private static RentalResponseDto MapToDto(Rental rental)
        {
            return new RentalResponseDto(
                rental.Id,
                rental.DailyRate,
                rental.DeliveryDriverId,
                rental.MotorcycleId,
                rental.StartDate,
                rental.EndDate,
                rental.ExpectedEndDate,
                rental.ActualEndDate
            );
        }
    }
}