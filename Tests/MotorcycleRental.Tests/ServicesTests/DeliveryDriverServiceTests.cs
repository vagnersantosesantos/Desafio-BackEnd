using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Repositories;
using MotorcycleRental.Application.Interfaces.Services;
using MotorcycleRental.Application.Services;

public class DeliveryDriverServiceTests
{
    private readonly Mock<IDeliveryDriverRepository> _driverRepoMock;
    private readonly Mock<IStorageService> _storageMock;
    private readonly Mock<ILogger<DeliveryDriverService>> _loggerMock;
    private readonly DeliveryDriverService _service;

    public DeliveryDriverServiceTests()
    {
        _driverRepoMock = new Mock<IDeliveryDriverRepository>();
        _storageMock = new Mock<IStorageService>();
        _loggerMock = new Mock<ILogger<DeliveryDriverService>>();
        _service = new DeliveryDriverService(_driverRepoMock.Object, _storageMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDriver_WhenValidData()
    {
        // Arrange
        var dto = new CreateDeliveryDriverDto
        {
            Id = "123",
            Name = "João",
            CNPJ = "12345678000190",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            LicenseNumber = "CNH123",
            LicenseType = LicenseType.A,
            LicenseImagePath = "path/to/image"
        };

        _driverRepoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((DeliveryDriver)null);
        _driverRepoMock.Setup(r => r.GetByCNPJAsync(dto.CNPJ)).ReturnsAsync((DeliveryDriver)null);
        _driverRepoMock.Setup(r => r.GetByLicenseNumberAsync(dto.LicenseNumber)).ReturnsAsync((DeliveryDriver)null);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.LicenseType, result.LicenseType);
        _driverRepoMock.Verify(r => r.AddAsync(It.IsAny<DeliveryDriver>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenDriverAlreadyExists()
    {
        // Arrange
        var dto = new CreateDeliveryDriverDto { Id = "123", CNPJ = "12345678000190", LicenseNumber = "CNH123", LicenseType = LicenseType.A };
        _driverRepoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(new DeliveryDriver());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDriver_WhenExists()
    {
        // Arrange
        var driver = new DeliveryDriver { Id = "123", Name = "João", LicenseType = LicenseType.B };
        _driverRepoMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(driver);

        // Act
        var result = await _service.GetByIdAsync("123");

        // Assert
        Assert.Equal(driver.Id, result.Id);
        Assert.Equal(driver.LicenseType, result.LicenseType);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
    {
        _driverRepoMock.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((DeliveryDriver)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync("999"));
    }

    [Fact]
    public async Task UpdateLicenseImageAsync_ShouldUpdate_WhenValidImage()
    {
        // Arrange
        var driver = new DeliveryDriver { Id = "123", LicenseImagePath = "old/path.png" };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("newimage.png");

        _driverRepoMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(driver);
        _storageMock.Setup(s => s.SaveFileAsync(It.IsAny<IFormFile>(), "license-images"))
                    .ReturnsAsync("new/path.png");

        // Act
        await _service.UpdateLicenseImageAsync("123", fileMock.Object);

        // Assert
        Assert.Equal("new/path.png", driver.LicenseImagePath);
        _storageMock.Verify(s => s.DeleteFileAsync("old/path.png"), Times.Once);
        _driverRepoMock.Verify(r => r.UpdateAsync(driver), Times.Once);
    }

    [Fact]
    public async Task UpdateLicenseImageAsync_ShouldThrow_WhenInvalidExtension()
    {
        var driver = new DeliveryDriver { Id = "123" };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("arquivo.pdf");

        _driverRepoMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(driver);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateLicenseImageAsync("123", fileMock.Object));
    }

    [Fact]
    public async Task UpdateLicenseImageAsync_ShouldThrow_WhenDriverNotFound()
    {
        var fileMock = new Mock<IFormFile>();
        _driverRepoMock.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((DeliveryDriver)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateLicenseImageAsync("999", fileMock.Object));
    }
}
