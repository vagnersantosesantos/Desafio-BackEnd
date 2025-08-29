using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotorcycleRental.Api.Controllers;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Services;

namespace MotorcycleRental.Tests.Controllers
{
    public class DeliveryDriversControllerTests
    {
        private readonly Mock<IDeliveryDriverService> _serviceMock;
        private readonly Mock<ILogger<DeliveryDriversController>> _loggerMock;
        private readonly DeliveryDriversController _controller;

        public DeliveryDriversControllerTests()
        {
            _serviceMock = new Mock<IDeliveryDriverService>();
            _loggerMock = new Mock<ILogger<DeliveryDriversController>>();
            _controller = new DeliveryDriversController(_serviceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateDeliveryDriver_ShouldReturnCreated_WhenSuccess()
        {
            // Arrange
            var request = new CreateDeliveryDriverDto { Name = "João", LicenseNumber = "123" };
            var response = new DeliveryDriverResponseDto { Id = "driver-1", Name = "João", LicenseNumber = "123" };

            _serviceMock.Setup(s => s.CreateAsync(request))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateDeliveryDriver(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<DeliveryDriverResponseDto>(createdResult.Value);
            Assert.Equal("driver-1", value.Id);
        }

        [Fact]
        public async Task CreateDeliveryDriver_ShouldReturnBadRequest_WhenInvalidOperation()
        {
            // Arrange
            var request = new CreateDeliveryDriverDto { Name = "João", LicenseNumber = "123" };

            _serviceMock.Setup(s => s.CreateAsync(request))
                        .ThrowsAsync(new InvalidOperationException("Motorista já cadastrado"));

            // Act
            var result = await _controller.CreateDeliveryDriver(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(badRequest.Value);
            Assert.Equal("Motorista já cadastrado", error.Message);
        }

        [Fact]
        public async Task GetDeliveryDriverById_ShouldReturnOk_WhenDriverExists()
        {
            // Arrange
            var response = new DeliveryDriverResponseDto { Id = "driver-1", Name = "João" };
            _serviceMock.Setup(s => s.GetByIdAsync("driver-1"))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.GetDeliveryDriverById("driver-1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<DeliveryDriverResponseDto>(okResult.Value);
            Assert.Equal("driver-1", value.Id);
        }

        [Fact]
        public async Task GetDeliveryDriverById_ShouldReturnNotFound_WhenDriverDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync("driver-1"))
                        .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetDeliveryDriverById("driver-1");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("Entregador não encontrado", error.Message);
        }

        [Fact]
        public async Task UploadLicenseImage_ShouldReturnOk_WhenUploadSuccess()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("cnh.png");

            _serviceMock.Setup(s => s.UpdateLicenseImageAsync("driver-1", fileMock.Object))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UploadLicenseImage("driver-1", fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UploadLicenseImage_ShouldReturnBadRequest_WhenFileIsNull()
        {
            // Act
            var result = await _controller.UploadLicenseImage("driver-1", null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponseDto>(badRequest.Value);
            Assert.Equal("Imagem da CNH é obrigatória", error.Message);
        }

        [Fact]
        public async Task UploadLicenseImage_ShouldReturnNotFound_WhenDriverDoesNotExist()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("cnh.png");

            _serviceMock.Setup(s => s.UpdateLicenseImageAsync("driver-1", fileMock.Object))
                        .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UploadLicenseImage("driver-1", fileMock.Object);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("Entregador não encontrado", error.Message);
        }
    }
}
