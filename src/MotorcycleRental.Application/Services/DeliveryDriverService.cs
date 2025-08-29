using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Repositories;
using MotorcycleRental.Application.Interfaces.Services;

namespace MotorcycleRental.Application.Services
{
    public class DeliveryDriverService : IDeliveryDriverService
    {
        private readonly IDeliveryDriverRepository _driverRepository;
        private readonly IStorageService _storageService;
        private readonly ILogger<DeliveryDriverService> _logger;

        public DeliveryDriverService(
            IDeliveryDriverRepository driverRepository,
            IStorageService storageService,
            ILogger<DeliveryDriverService> logger)
        {
            _driverRepository = driverRepository;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<DeliveryDriverResponseDto> CreateAsync(CreateDeliveryDriverDto request)
        {
            _logger.LogInformation("Creating delivery driver with ID: {Id}", request.Id);

            // Validar se já existe
            var existing = await _driverRepository.GetByIdAsync(request.Id);
            if (existing != null)
                throw new InvalidOperationException("Dados inválidos");

            // Validar CNPJ único
            var existingByCnpj = await _driverRepository.GetByCNPJAsync(request.CNPJ);
            if (existingByCnpj != null)
                throw new InvalidOperationException("Dados inválidos");

            // Validar CNH única
            var existingByLicense = await _driverRepository.GetByLicenseNumberAsync(request.LicenseNumber);
            if (existingByLicense != null)
                throw new InvalidOperationException("Dados inválidos");

            // Validar tipo de CNH
            if (!IsValidLicenseType(request.LicenseType))
                throw new InvalidOperationException("Dados inválidos");

            var driver = new DeliveryDriver
            {
                Id = request.Id,
                Name = request.Name,
                CNPJ = request.CNPJ,
                BirthDate = request.BirthDate,
                LicenseNumber = request.LicenseNumber,
                LicenseType = request.LicenseType,
                LicenseImagePath = request.LicenseImagePath
            };

            await _driverRepository.AddAsync(driver);

            return MapToResponse(driver);
        }

        public async Task<DeliveryDriverResponseDto> GetByIdAsync(string id)
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null)
                throw new KeyNotFoundException("Entregador não encontrado");

            return MapToResponse(driver);
        }

        public async Task UpdateLicenseImageAsync(string id, IFormFile imageFile)
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null)
                throw new KeyNotFoundException("Entregador não encontrado");

            // Validar formato do arquivo
            var allowedExtensions = new[] { ".png", ".bmp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new InvalidOperationException("Dados inválidos");

            // Deletar imagem anterior se existe
            if (!string.IsNullOrEmpty(driver.LicenseImagePath))
            {
                await _storageService.DeleteFileAsync(driver.LicenseImagePath);
            }

            // Salvar nova imagem
            var imagePath = await _storageService.SaveFileAsync(imageFile, "license-images");
            driver.LicenseImagePath = imagePath;

            await _driverRepository.UpdateAsync(driver);
        }

        private static bool IsValidLicenseType(LicenseType licenseType)
        {
            return licenseType.ToString() == "A" || licenseType.ToString() == "B" || licenseType.ToString() == "AB";
        }

        private static DeliveryDriverResponseDto MapToResponse(DeliveryDriver driver)
        {
            return new DeliveryDriverResponseDto
            {
                Id = driver.Id,
                Name = driver.Name,
                CNPJ = driver.CNPJ,
                BirthDate = driver.BirthDate,
                LicenseNumber = driver.LicenseNumber,
                LicenseType = driver.LicenseType,
                LicenseImagePath = driver.LicenseImagePath
            };
        }
    }
}
