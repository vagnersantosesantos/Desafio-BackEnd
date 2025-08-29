using Domain.Entities;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using MotorcycleRental.Application.Interfaces.Repositories;

namespace MotorcycleRental.Infrastructure.Repositories
{
    public class DeliveryDriverRepository : IDeliveryDriverRepository
    {
        private readonly DataContext _context;

        public DeliveryDriverRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<DeliveryDriver?> GetByIdAsync(string id)
        {
            return await _context.DeliveryDrivers.FindAsync(id);
        }

        public async Task<DeliveryDriver?> GetByCNPJAsync(string cnpj)
        {
            return await _context.DeliveryDrivers
                .FirstOrDefaultAsync(d => d.CNPJ == cnpj);
        }

        public async Task<DeliveryDriver?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _context.DeliveryDrivers
                .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
        }

        public async Task<IEnumerable<DeliveryDriver>> GetAllAsync()
        {
            return await _context.DeliveryDrivers
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<DeliveryDriver> AddAsync(DeliveryDriver deliveryDriver)
        {
            _context.DeliveryDrivers.Add(deliveryDriver);
            await _context.SaveChangesAsync();
            return deliveryDriver;
        }

        public async Task UpdateAsync(DeliveryDriver deliveryDriver)
        {
            _context.DeliveryDrivers.Update(deliveryDriver);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.DeliveryDrivers
                .AnyAsync(d => d.Id == id);
        }
    }
}
