using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Repositories;
using MotorcycleRental.Application.Interfaces.Services;
using MotorcycleRental.Application.Services;

namespace MotorcycleRental.Tests.Services
{
    public class MotorcycleServiceTests
    {
        private readonly Mock<IMotorcycleRepository> _repositoryMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerMock;
        private readonly Mock<ILogger<MotorcycleService>> _loggerMock;
        private readonly MotorcycleService _service;

        public MotorcycleServiceTests()
        {
            _repositoryMock = new Mock<IMotorcycleRepository>();
            _messageBrokerMock = new Mock<IMessageBrokerService>();
            _loggerMock = new Mock<ILogger<MotorcycleService>>();

            _service = new MotorcycleService(
                _repositoryMock.Object,
                _messageBrokerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateMotorcycle_WhenValidRequest()
        {
            // Arrange
            var request = new CreateMotorcycleDto("123", 2024, "Honda", "ABC-1234");

            _repositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((Motorcycle?)null);
            _repositoryMock.Setup(r => r.GetByLicensePlateAsync(request.LicensePlate)).ReturnsAsync((Motorcycle?)null);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.Equal(request.Id, result.Id);
            Assert.Equal(request.Model, result.Model);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Motorcycle>()), Times.Once);
            _messageBrokerMock.Verify(m => m.PublishMotorcycleRegisteredAsync(It.IsAny<Motorcycle>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenMotorcycleAlreadyExists()
        {
            // Arrange
            var request = new CreateMotorcycleDto("123", 2024, "Honda", "ABC-1234");

            _repositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(new Motorcycle { Id = request.Id });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenLicensePlateAlreadyExists()
        {
            var request = new CreateMotorcycleDto("123", 2024, "Honda", "ABC-1234");

            _repositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((Motorcycle?)null);
            _repositoryMock.Setup(r => r.GetByLicensePlateAsync(request.LicensePlate))
                .ReturnsAsync(new Motorcycle { Id = "999", LicensePlate = request.LicensePlate });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMotorcycle_WhenExists()
        {
            var moto = new Motorcycle { Id = "123", Model = "Yamaha", Year = 2022, LicensePlate = "XYZ-9876" };
            _repositoryMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(moto);

            var result = await _service.GetByIdAsync("123");

            Assert.Equal("123", result.Id);
            Assert.Equal("Yamaha", result.Model);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync("notfound")).ReturnsAsync((Motorcycle?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync("notfound"));
        }

        [Fact]
        public async Task UpdateLicensePlateAsync_ShouldUpdate_WhenValid()
        {
            var moto = new Motorcycle { Id = "123", LicensePlate = "OLD-1111" };
            _repositoryMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(moto);
            _repositoryMock.Setup(r => r.GetByLicensePlateAsync("NEW-2222")).ReturnsAsync((Motorcycle?)null);

            var request = new UpdateMotorcycleLicensePlateDto { LicensePlate = "NEW-2222" };

            var result = await _service.UpdateLicensePlateAsync("123", request);

            Assert.Equal("NEW-2222", result.LicensePlate);
            _repositoryMock.Verify(r => r.UpdateAsync(moto), Times.Once);
        }

        [Fact]
        public async Task UpdateLicensePlateAsync_ShouldThrow_WhenPlateAlreadyExists()
        {
            var moto = new Motorcycle { Id = "123", LicensePlate = "OLD-1111" };
            _repositoryMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(moto);
            _repositoryMock.Setup(r => r.GetByLicensePlateAsync("EXIST-9999"))
                .ReturnsAsync(new Motorcycle { Id = "456", LicensePlate = "EXIST-9999" });

            var request = new UpdateMotorcycleLicensePlateDto { LicensePlate = "EXIST-9999" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateLicensePlateAsync("123", request));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenNoRentals()
        {
            var moto = new Motorcycle { Id = "123" };
            _repositoryMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(moto);
            _repositoryMock.Setup(r => r.HasRentalsAsync("123")).ReturnsAsync(false);

            await _service.DeleteAsync("123");

            _repositoryMock.Verify(r => r.DeleteAsync(moto), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenHasRentals()
        {
            var moto = new Motorcycle { Id = "123" };
            _repositoryMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(moto);
            _repositoryMock.Setup(r => r.HasRentalsAsync("123")).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync("123"));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync("notfound")).ReturnsAsync((Motorcycle?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync("notfound"));
        }
    }
}
