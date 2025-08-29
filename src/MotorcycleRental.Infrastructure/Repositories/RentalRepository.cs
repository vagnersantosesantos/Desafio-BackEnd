using Domain.Entities;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using MotorcycleRental.Application.Interfaces.Repositories;

namespace MotorcycleRental.Infrastructure.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly DataContext _context;

        public RentalRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Rental?> GetByIdAsync(string id)
        {
            return await _context.Rentals
                .Include(r => r.Motorcycle)
                .Include(r => r.DeliveryDriver)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(string motorcycleId)
        {
            return await _context.Rentals
                .Where(r => r.MotorcycleId == motorcycleId)
                .Include(r => r.DeliveryDriver)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetByDeliveryDriverIdAsync(string deliveryDriverId)
        {
            return await _context.Rentals
                .Where(r => r.DeliveryDriverId == deliveryDriverId)
                .Include(r => r.Motorcycle)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetActiveRentalsByMotorcycleIdAsync(string motorcycleId)
        {
            return await _context.Rentals
                .Where(r => r.MotorcycleId == motorcycleId &&
                           r.ActualEndDate == null &&
                           r.EndDate >= DateTime.UtcNow.Date)
                .ToListAsync();
        }

        public async Task<Rental> AddAsync(Rental rental)
        {
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();
            return rental;
        }

        public async Task UpdateAsync(Rental rental)
        {
            _context.Rentals.Update(rental);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Rentals
                .AnyAsync(r => r.Id == id);
        }
    }
}
