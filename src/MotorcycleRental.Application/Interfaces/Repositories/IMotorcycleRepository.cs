using Domain.Entities;

namespace MotorcycleRental.Application.Interfaces.Repositories
{
    public interface IMotorcycleRepository
    {
        Task<Motorcycle?> GetByIdAsync(string id);
        Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate);
        Task<IEnumerable<Motorcycle>> GetAllAsync();
        Task<IEnumerable<Motorcycle>> GetFilteredAsync(string licensePlateFilter);
        Task<Motorcycle> AddAsync(Motorcycle motorcycle);
        Task UpdateAsync(Motorcycle motorcycle);
        Task DeleteAsync(Motorcycle motorcycle);
        Task<bool> HasRentalsAsync(string motorcycleId);
    }
}
