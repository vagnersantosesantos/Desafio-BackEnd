using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotorcycleRental.Api.Controllers;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Services;
using System.Numerics;

namespace MotorcycleRental.Tests.Controllers
{
    public class RentalsControllerTests
    {
        private readonly Mock<IRentalService> _rentalServiceMock;
        private readonly Mock<ILogger<RentalsController>> _loggerMock;
        private readonly RentalsController _controller;

        public RentalsControllerTests()
        {
            _rentalServiceMock = new Mock<IRentalService>();
            _loggerMock = new Mock<ILogger<RentalsController>>();
            _controller = new RentalsController(_rentalServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateRental_ShouldReturnCreated_WhenSuccessful()
        {
            // Arrange
            var request = new CreateRentalDto { DeliveryDriverId = "entregador123", MotorcycleId = "moto123", StartDate = DateTime.Now,  EndDate = DateTime.Now.AddDays(1), ExpectedEndDate = DateTime.Now.AddDays(1), Plan = 7 };

            var response = new RentalResponseDto(
                    id: "123",
                    dailyRate: 100.0m,
                    deliveryDriverId: "entregador123",
                    motorcycleId: "moto123",
                    startDate: DateTime.Now,
                    endDate: DateTime.Now.AddDays(1),
                    expectedEndDate: DateTime.Now.AddDays(1),
                    actualEndDate: null
                );

            _rentalServiceMock.Setup(s => s.CreateRentalAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateRental(request);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var rental = Assert.IsType<RentalResponseDto>(createdAt.Value);
            Assert.Equal("123", rental.Id);
        }

        [Fact]
        public async Task CreateRental_ShouldReturnNotFound_WhenRentalNotFound()
        {
            var request = new CreateRentalDto();
            _rentalServiceMock.Setup(s => s.CreateRentalAsync(request))
                .ThrowsAsync(new KeyNotFoundException("Moto não disponível"));

            var result = await _controller.CreateRental(request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("Moto não disponível", error.Message);
        }

        [Fact]
        public async Task CreateRental_ShouldReturnBadRequest_WhenInvalidOperation()
        {
            var request = new CreateRentalDto();
            _rentalServiceMock.Setup(s => s.CreateRentalAsync(request))
                .ThrowsAsync(new InvalidOperationException("Locação inválida"));

            var result = await _controller.CreateRental(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(badRequest.Value);
            Assert.Equal("Locação inválida", error.Message);
        }

        [Fact]
        public async Task GetRentalById_ShouldReturnOk_WhenRentalExists()
        {
            var response = new RentalResponseDto(
                    id: "123",
                    dailyRate: 100.0m,
                    deliveryDriverId: "entregador123",
                    motorcycleId: "moto123",
                    startDate: DateTime.Now,
                    endDate: DateTime.Now.AddDays(1),
                    expectedEndDate: DateTime.Now.AddDays(1),
                    actualEndDate: null
                );
            
            _rentalServiceMock.Setup(s => s.GetByIdAsync("123")).ReturnsAsync(response);

            var result = await _controller.GetRentalById("123");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var rental = Assert.IsType<RentalResponseDto>(okResult.Value);
            Assert.Equal("123", rental.Id);
        }

        [Fact]
        public async Task GetRentalById_ShouldReturnNotFound_WhenRentalDoesNotExist()
        {
            _rentalServiceMock.Setup(s => s.GetByIdAsync("999"))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.GetRentalById("999");

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("Locação não encontrada", error.Message);
        }

        [Fact]
        public async Task ReturnRental_DeveRetornarOk_QuandoSucesso()
        {
            // Arrange
            var rentalId = "123";
            var returnDate = DateTime.Now.AddDays(2);

            _rentalServiceMock
                .Setup(s => s.ReturnRentalAsync(rentalId, returnDate))
                .ReturnsAsync(new ReturnRentalTotalCoastDto(200.0m));

            // Act
            var result = await _controller.ReturnRental(rentalId, returnDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ReturnRental_DeveRetornarNotFound_QuandoLocacaoNaoExiste()
        {
            // Arrange
            var rentalId = "nao_existe";
            var returnDate = DateTime.Now;

            _rentalServiceMock
                .Setup(s => s.ReturnRentalAsync(rentalId, returnDate))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.ReturnRental(rentalId, returnDate);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponseDto>(notFoundResult.Value);
            Assert.Equal("Locação não encontrada", error.Message);
        }

        [Fact]
        public async Task ReturnRental_DeveRetornarBadRequest_QuandoErroDeNegocio()
        {
            // Arrange
            var rentalId = "123";
            var returnDate = DateTime.Now;

            _rentalServiceMock
                .Setup(s => s.ReturnRentalAsync(rentalId, returnDate))
                .ThrowsAsync(new InvalidOperationException("Já devolvida"));

            // Act
            var result = await _controller.ReturnRental(rentalId, returnDate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponseDto>(badRequestResult.Value);
            Assert.Equal("Já devolvida", error.Message);
        }
    }
}
