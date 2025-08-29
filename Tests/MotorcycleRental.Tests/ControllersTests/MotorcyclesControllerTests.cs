using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotorcycleRental.Api.Controllers;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Services;

namespace MotorcycleRental.Tests.Controllers
{
    public class MotorcyclesControllerTests
    {
        private readonly Mock<IMotorcycleService> _serviceMock;
        private readonly Mock<ILogger<MotorcyclesController>> _loggerMock;
        private readonly MotorcyclesController _controller;

        public MotorcyclesControllerTests()
        {
            _serviceMock = new Mock<IMotorcycleService>();
            _loggerMock = new Mock<ILogger<MotorcyclesController>>();
            _controller = new MotorcyclesController(_serviceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateMotorcycle_ShouldReturnCreated_WhenSuccess()
        {
            // Arrange
            var request = new CreateMotorcycleDto("moto-1", 2023, "Honda", "ABC1234");
            var response = new MotorcycleResponseDto { Id = "moto-1", LicensePlate = "ABC1234", Model = "Honda" };

            _serviceMock.Setup(s => s.CreateAsync(request))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateMotorcycle(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<MotorcycleResponseDto>(createdResult.Value);
            Assert.Equal("moto-1", value.Id);
        }

        [Fact]
        public async Task CreateMotorcycle_ShouldReturnBadRequest_WhenInvalidOperation()
        {
            // Arrange
            var request = new CreateMotorcycleDto("moto-1", 2023, "Honda", "ABC1234");

            _serviceMock.Setup(s => s.CreateAsync(request))
                        .ThrowsAsync(new InvalidOperationException("Placa já cadastrada"));

            // Act
            var result = await _controller.CreateMotorcycle(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(badRequest.Value);
            Assert.Equal("Placa já cadastrada", error.Message);
        }

        [Fact]
        public async Task GetMotorcycles_ShouldReturnOk_WithList()
        {
            // Arrange
            var list = new List<MotorcycleResponseDto>
            {
                new MotorcycleResponseDto { Id = "moto-1", LicensePlate = "ABC1234" },
                new MotorcycleResponseDto { Id = "moto-2", LicensePlate = "XYZ5678" }
            };

            _serviceMock.Setup(s => s.GetAllAsync(null))
                        .ReturnsAsync(list);

            // Act
            var result = await _controller.GetMotorcycles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<IEnumerable<MotorcycleResponseDto>>(okResult.Value);
            Assert.Collection(value,
                item => Assert.Equal("moto-1", item.Id),
                item => Assert.Equal("moto-2", item.Id));
        }

        [Fact]
        public async Task GetMotorcycleById_ShouldReturnOk_WhenExists()
        {
            // Arrange
            var response = new MotorcycleResponseDto { Id = "moto-1", LicensePlate = "ABC1234" };
            _serviceMock.Setup(s => s.GetByIdAsync("moto-1"))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.GetMotorcycleById("moto-1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<MotorcycleResponseDto>(okResult.Value);
            Assert.Equal("moto-1", value.Id);
        }

        [Fact]
        public async Task GetMotorcycleById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync("moto-1"))
                        .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetMotorcycleById("moto-1");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("Moto não encontrada", error.Message);
        }

        [Fact]
        public async Task UpdateLicensePlate_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var request = new UpdateMotorcycleLicensePlateDto { LicensePlate = "XYZ5678" };
            var response = new MotorcycleResponseDto { Id = "moto-1", LicensePlate = "XYZ5678" };

            _serviceMock.Setup(s => s.UpdateLicensePlateAsync("moto-1", request))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateLicensePlate("moto-1", request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<MotorcycleResponseDto>(okResult.Value);
            Assert.Equal("XYZ5678", value.LicensePlate);
        }

        [Fact]
        public async Task UpdateLicensePlate_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            var request = new UpdateMotorcycleLicensePlateDto { LicensePlate = "XYZ5678" };

            _serviceMock.Setup(s => s.UpdateLicensePlateAsync("moto-1", request))
                        .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateLicensePlate("moto-1", request);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("Moto não encontrada", error.Message);
        }

        [Fact]
        public async Task UpdateLicensePlate_ShouldReturnBadRequest_WhenInvalidOperation()
        {
            // Arrange
            var request = new UpdateMotorcycleLicensePlateDto { LicensePlate = "XYZ5678" };

            _serviceMock.Setup(s => s.UpdateLicensePlateAsync("moto-1", request))
                        .ThrowsAsync(new InvalidOperationException("Placa já em uso"));

            // Act
            var result = await _controller.UpdateLicensePlate("moto-1", request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(badRequest.Value);
            Assert.Equal("Placa já em uso", error.Message);
        }

        [Fact]
        public async Task DeleteMotorcycle_ShouldReturnNoContent_WhenSuccess()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync("moto-1"))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteMotorcycle("moto-1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMotorcycle_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync("moto-1"))
                        .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteMotorcycle("moto-1");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("Moto não encontrada", error.Message);
        }

        [Fact]
        public async Task DeleteMotorcycle_ShouldReturnBadRequest_WhenInvalidOperation()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync("moto-1"))
                        .ThrowsAsync(new InvalidOperationException("Moto não pode ser removida"));

            // Act
            var result = await _controller.DeleteMotorcycle("moto-1");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponseDto>(badRequest.Value);
            Assert.Equal("Moto não pode ser removida", error.Message);
        }
    }
}
