using Domain.Entities;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using MotorcycleRental.Application.Interfaces.Repositories;

namespace MotorcycleRental.Infrastructure.Repositories
{
    public class NotificationLogRepository : INotificationLogRepository
    {
        private readonly DataContext _context;

        public NotificationLogRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<NotificationLog> AddAsync(NotificationLog notificationLog)
        {
            _context.NotificationLogs.Add(notificationLog);
            await _context.SaveChangesAsync();
            return notificationLog;
        }

        public async Task<IEnumerable<NotificationLog>> GetByMotorcycleIdAsync(string motorcycleId)
        {
            return await _context.NotificationLogs
                .Where(n => n.MotorcycleId == motorcycleId)
                .Include(n => n.Motorcycle)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<NotificationLog>> GetAllAsync()
        {
            return await _context.NotificationLogs
                .Include(n => n.Motorcycle)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
