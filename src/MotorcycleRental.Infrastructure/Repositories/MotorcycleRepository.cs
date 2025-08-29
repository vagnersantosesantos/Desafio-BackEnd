using Domain.Entities;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using MotorcycleRental.Application.Interfaces.Repositories;

namespace MotorcycleRental.Infrastructure.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly DataContext _context;

        public MotorcycleRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Motorcycle?> GetByIdAsync(string id)
        {
            return await _context.Motorcycles.FindAsync(id);
        }

        public async Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate)
        {
            return await _context.Motorcycles
                .FirstOrDefaultAsync(m => m.LicensePlate == licensePlate);
        }

        public async Task<IEnumerable<Motorcycle>> GetAllAsync()
        {
            return await _context.Motorcycles
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Motorcycle>> GetFilteredAsync(string licensePlateFilter)
        {
            return await _context.Motorcycles
                .Where(m => m.LicensePlate.Contains(licensePlateFilter))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Motorcycle> AddAsync(Motorcycle motorcycle)
        {
            _context.Motorcycles.Add(motorcycle);
            await _context.SaveChangesAsync();
            return motorcycle;
        }

        public async Task UpdateAsync(Motorcycle motorcycle)
        {
            _context.Motorcycles.Update(motorcycle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Motorcycle motorcycle)
        {
            _context.Motorcycles.Remove(motorcycle);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasRentalsAsync(string motorcycleId)
        {
            return await _context.Rentals
                .AnyAsync(r => r.MotorcycleId == motorcycleId);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Motorcycles
                .AnyAsync(m => m.Id == id);
        }
    }
}
