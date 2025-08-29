using MotorcycleRental.Application.DTOs;

namespace MotorcycleRental.Application.Interfaces.Services
{
    public interface IMotorcycleService
    {
        Task<MotorcycleResponseDto> CreateAsync(CreateMotorcycleDto dto);
        Task<IEnumerable<MotorcycleResponseDto>> GetAllAsync(string? licensePlateFilter = null);
        Task<MotorcycleResponseDto> GetByIdAsync(string id);
        Task<MotorcycleResponseDto> UpdateLicensePlateAsync(string id, UpdateMotorcycleLicensePlateDto dto);
        Task DeleteAsync(string id);
    }
}