using Domain.Entities;

namespace MotorcycleRental.Application.Interfaces.Repositories
{
    public interface INotificationLogRepository
    {
        Task<NotificationLog> AddAsync(NotificationLog notificationLog);
        Task<IEnumerable<NotificationLog>> GetByMotorcycleIdAsync(string motorcycleId);
        Task<IEnumerable<NotificationLog>> GetAllAsync();
    }
}
