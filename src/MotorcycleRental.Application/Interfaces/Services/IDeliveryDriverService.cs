using Microsoft.AspNetCore.Http;
using MotorcycleRental.Application.DTOs;

namespace MotorcycleRental.Application.Interfaces.Services
{
    public interface IDeliveryDriverService
    {
        Task<DeliveryDriverResponseDto> CreateAsync(CreateDeliveryDriverDto dto);
        Task<DeliveryDriverResponseDto> GetByIdAsync(string id);
        Task UpdateLicenseImageAsync(string id, IFormFile imageFile);
    }
}
