using Domain.Entities;
using Domain.Enum;
using Microsoft.Extensions.Logging;
using Moq;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Repositories;
using MotorcycleRental.Application.Services;

namespace MotorcycleRental.Tests.Services
{
    public class RentalServiceTests
    {
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
        private readonly Mock<IDeliveryDriverRepository> _driverRepositoryMock;
        private readonly Mock<ILogger<RentalService>> _loggerMock;
        private readonly RentalService _service;

        public RentalServiceTests()
        {
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
            _driverRepositoryMock = new Mock<IDeliveryDriverRepository>();
            _loggerMock = new Mock<ILogger<RentalService>>();

            _service = new RentalService(
                _rentalRepositoryMock.Object,
                _motorcycleRepositoryMock.Object,
                _driverRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateRentalAsync_ShouldCreateRental_WhenValid()
        {
            var dto = new CreateRentalDto
            {
                MotorcycleId = "moto1",
                DeliveryDriverId = "driver1",
                Plan = 7
            };

            _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(dto.MotorcycleId))
                .ReturnsAsync(new Motorcycle { Id = dto.MotorcycleId });
            _rentalRepositoryMock.Setup(r => r.GetActiveRentalsByMotorcycleIdAsync(dto.MotorcycleId))
                .ReturnsAsync(new List<Rental>());
            _driverRepositoryMock.Setup(r => r.GetByIdAsync(dto.DeliveryDriverId))
                .ReturnsAsync(new DeliveryDriver { Id = dto.DeliveryDriverId, LicenseType = LicenseType.A });

            var result = await _service.CreateRentalAsync(dto);

            Assert.Equal(dto.MotorcycleId, result.MotorcycleId);
            Assert.Equal(dto.DeliveryDriverId, result.DeliveryDriverId);
            _rentalRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Rental>()), Times.Once);
        }

        [Fact]
        public async Task CreateRentalAsync_ShouldThrow_WhenMotorcycleNotFound()
        {
            var dto = new CreateRentalDto { MotorcycleId = "notfound", DeliveryDriverId = "driver1", Plan = 7 };

            _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(dto.MotorcycleId)).ReturnsAsync((Motorcycle?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateRentalAsync(dto));
        }

        [Fact]
        public async Task CreateRentalAsync_ShouldThrow_WhenMotorcycleAlreadyRented()
        {
            var dto = new CreateRentalDto { MotorcycleId = "moto1", DeliveryDriverId = "driver1", Plan = 7 };

            _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(dto.MotorcycleId)).ReturnsAsync(new Motorcycle());
            _rentalRepositoryMock.Setup(r => r.GetActiveRentalsByMotorcycleIdAsync(dto.MotorcycleId))
                .ReturnsAsync(new List<Rental> { new Rental() });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(dto));
        }

        [Fact]
        public async Task CreateRentalAsync_ShouldThrow_WhenDriverNotFound()
        {
            var dto = new CreateRentalDto { MotorcycleId = "moto1", DeliveryDriverId = "driver404", Plan = 7 };

            _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(dto.MotorcycleId)).ReturnsAsync(new Motorcycle());
            _rentalRepositoryMock.Setup(r => r.GetActiveRentalsByMotorcycleIdAsync(dto.MotorcycleId))
                .ReturnsAsync(new List<Rental>());
            _driverRepositoryMock.Setup(r => r.GetByIdAsync(dto.DeliveryDriverId)).ReturnsAsync((DeliveryDriver?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateRentalAsync(dto));
        }

        [Fact]
        public async Task CreateRentalAsync_ShouldThrow_WhenDriverHasInvalidLicense()
        {
            var dto = new CreateRentalDto { MotorcycleId = "moto1", DeliveryDriverId = "driver1", Plan = 7 };

            _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(dto.MotorcycleId)).ReturnsAsync(new Motorcycle());
            _rentalRepositoryMock.Setup(r => r.GetActiveRentalsByMotorcycleIdAsync(dto.MotorcycleId))
                .ReturnsAsync(new List<Rental>());
            _driverRepositoryMock.Setup(r => r.GetByIdAsync(dto.DeliveryDriverId))
                .ReturnsAsync(new DeliveryDriver { Id = dto.DeliveryDriverId, LicenseType = LicenseType.B });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(dto));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRental_WhenExists()
        {
            var rental = new Rental { Id = "rental1", MotorcycleId = "moto1", DeliveryDriverId = "driver1" };
            _rentalRepositoryMock.Setup(r => r.GetByIdAsync("rental1")).ReturnsAsync(rental);

            var result = await _service.GetByIdAsync("rental1");

            Assert.Equal("rental1", result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _rentalRepositoryMock.Setup(r => r.GetByIdAsync("notfound")).ReturnsAsync((Rental?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync("notfound"));
        }

        [Fact]
        public async Task ReturnRentalAsync_ShouldCalculateCost_WhenValid()
        {
            var rental = new Rental
            {
                Id = "rental1",
                StartDate = DateTime.UtcNow.AddDays(-7),
                Plan = RentalPlan.SevenDays,
                DailyRate = 30m
            };
            _rentalRepositoryMock.Setup(r => r.GetByIdAsync("rental1")).ReturnsAsync(rental);

            var returnDate = DateTime.UtcNow.Date;
            var result = await _service.ReturnRentalAsync("rental1", returnDate);

            Assert.True(result.TotalCost > 0);
        }

        [Fact]
        public async Task ReturnRentalAsync_ShouldThrow_WhenRentalNotFound()
        {
            _rentalRepositoryMock.Setup(r => r.GetByIdAsync("notfound")).ReturnsAsync((Rental?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ReturnRentalAsync("notfound", DateTime.UtcNow));
        }

        [Fact]
        public async Task ReturnRentalAsync_ShouldThrow_WhenRentalAlreadyCompleted()
        {
            var rental = new Rental
            {
                Id = "rental1",
                StartDate = DateTime.UtcNow,
                Plan = RentalPlan.SevenDays,
                ActualEndDate = DateTime.UtcNow
            };
            _rentalRepositoryMock.Setup(r => r.GetByIdAsync("rental1")).ReturnsAsync(rental);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ReturnRentalAsync("rental1", DateTime.UtcNow));
        }

        [Fact]
        public async Task ReturnRentalAsync_ShouldApplyPenalty_WhenReturnedEarly()
        {
            var rental = new Rental
            {
                Id = "rental1",
                StartDate = DateTime.UtcNow.AddDays(-2),
                Plan = RentalPlan.SevenDays,
                DailyRate = 30m
            };
            _rentalRepositoryMock.Setup(r => r.GetByIdAsync("rental1")).ReturnsAsync(rental);

            var returnDate = rental.StartDate.AddDays(2);
            var result = await _service.ReturnRentalAsync("rental1", returnDate);

            Assert.True(result.TotalCost > 0);
        }

        [Fact]
        public async Task ReturnRentalAsync_ShouldApplyExtraCost_WhenReturnedLate()
        {
            var rental = new Rental
            {
                Id = "rental1",
                StartDate = DateTime.UtcNow.AddDays(-10),
                Plan = RentalPlan.SevenDays,
                DailyRate = 30m
            };
            _rentalRepositoryMock.Setup(r => r.GetByIdAsync("rental1")).ReturnsAsync(rental);

            var returnDate = rental.StartDate.AddDays(10);
            var result = await _service.ReturnRentalAsync("rental1", returnDate);

            Assert.True(result.TotalCost > 0);
        }
    }
}
