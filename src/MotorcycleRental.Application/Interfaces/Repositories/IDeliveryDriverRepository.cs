using Domain.Entities;

namespace MotorcycleRental.Application.Interfaces.Repositories
{
    public interface IDeliveryDriverRepository
    {
        Task<DeliveryDriver?> GetByIdAsync(string id);
        Task<DeliveryDriver?> GetByCNPJAsync(string cnpj);
        Task<DeliveryDriver?> GetByLicenseNumberAsync(string licenseNumber);
        Task<IEnumerable<DeliveryDriver>> GetAllAsync();
        Task<DeliveryDriver> AddAsync(DeliveryDriver deliveryDriver);
        Task UpdateAsync(DeliveryDriver deliveryDriver);
        Task<bool> ExistsAsync(string id);
    }
}
