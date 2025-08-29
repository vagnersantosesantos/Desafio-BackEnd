using Domain.Entities;

namespace MotorcycleRental.Application.Interfaces.Repositories
{
    public interface IRentalRepository
    {
        Task<Rental?> GetByIdAsync(string id);
        Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(string motorcycleId);
        Task<IEnumerable<Rental>> GetByDeliveryDriverIdAsync(string deliveryDriverId);
        Task<IEnumerable<Rental>> GetActiveRentalsByMotorcycleIdAsync(string motorcycleId);
        Task<Rental> AddAsync(Rental rental);
        Task UpdateAsync(Rental rental);
        Task<bool> ExistsAsync(string id);
    }
}
