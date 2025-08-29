using Domain.Entities;

namespace MotorcycleRental.Application.Interfaces.Services
{
    public interface IMessageBrokerService
    {
        Task PublishMotorcycleRegisteredAsync(Motorcycle motorcycle);
    }
}
