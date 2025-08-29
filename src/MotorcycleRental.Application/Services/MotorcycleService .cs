using Domain.Entities;
using Microsoft.Extensions.Logging;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Repositories;
using MotorcycleRental.Application.Interfaces.Services;

namespace MotorcycleRental.Application.Services
{
    public class MotorcycleService : IMotorcycleService
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IMessageBrokerService _messageBroker;
        private readonly ILogger<MotorcycleService> _logger;

        public MotorcycleService(
            IMotorcycleRepository motorcycleRepository,
            IMessageBrokerService messageBroker,
            ILogger<MotorcycleService> logger)
        {
            _motorcycleRepository = motorcycleRepository;
            _messageBroker = messageBroker;
            _logger = logger;
        }

        public async Task<MotorcycleResponseDto> CreateAsync(CreateMotorcycleDto request)
        {
            _logger.LogInformation("Creating motorcycle with ID: {Id}", request.Id);

            // Validar se já existe
            var existing = await _motorcycleRepository.GetByIdAsync(request.Id);
            if (existing != null)
                throw new InvalidOperationException("Dados inválidos");

            // Validar placa única
            var existingByPlate = await _motorcycleRepository.GetByLicensePlateAsync(request.LicensePlate);
            if (existingByPlate != null)
                throw new InvalidOperationException("Dados inválidos");

            var motorcycle = new Motorcycle
            {
                Id = request.Id,
                Year = request.Year,
                Model = request.Model,
                LicensePlate = request.LicensePlate
            };

            await _motorcycleRepository.AddAsync(motorcycle);

            // Publicar evento
            await _messageBroker.PublishMotorcycleRegisteredAsync(motorcycle);

            return MapToResponse(motorcycle);
        }

        public async Task<IEnumerable<MotorcycleResponseDto>> GetAllAsync(string? licensePlateFilter = null)
        {
            IEnumerable<Motorcycle> motorcycles;

            if (!string.IsNullOrWhiteSpace(licensePlateFilter))
                motorcycles = await _motorcycleRepository.GetFilteredAsync(licensePlateFilter);
            else
                motorcycles = await _motorcycleRepository.GetAllAsync();

            return motorcycles.Select(MapToResponse);
        }

        public async Task<MotorcycleResponseDto> GetByIdAsync(string id)
        {
            var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
            if (motorcycle == null)
                throw new KeyNotFoundException("Moto não encontrada");

            return MapToResponse(motorcycle);
        }

        public async Task<MotorcycleResponseDto> UpdateLicensePlateAsync(string id, UpdateMotorcycleLicensePlateDto request)
        {
            var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
            if (motorcycle == null)
                throw new KeyNotFoundException("Moto não encontrada");

            // Validar se nova placa já existe
            var existingWithPlate = await _motorcycleRepository.GetByLicensePlateAsync(request.LicensePlate);
            if (existingWithPlate != null && existingWithPlate.Id != id)
                throw new InvalidOperationException("Dados inválidos");

            motorcycle.LicensePlate = request.LicensePlate;
            await _motorcycleRepository.UpdateAsync(motorcycle);

            return MapToResponse(motorcycle);
        }

        public async Task DeleteAsync(string id)
        {
            var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
            if (motorcycle == null)
                throw new KeyNotFoundException("Moto não encontrada");

            // Verificar se tem locações
            var hasRentals = await _motorcycleRepository.HasRentalsAsync(id);
            if (hasRentals)
                throw new InvalidOperationException("Dados inválidos");

            await _motorcycleRepository.DeleteAsync(motorcycle);
        }

        private static MotorcycleResponseDto MapToResponse(Motorcycle motorcycle)
        {
            return new MotorcycleResponseDto
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year,
                Model = motorcycle.Model,
                LicensePlate = motorcycle.LicensePlate
            };
        }
    }
}