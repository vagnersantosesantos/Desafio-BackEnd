using Domain.Entities;
using Infra.Data.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MotorcycleRental.Infrastructure.Services
{
    public class RabbitMQService : IMessageBrokerService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitMQService> _logger;
        private const string ExchangeName = "motorcycle.events";
        private const string QueueName = "motorcycle.registered";
        private const string RoutingKey = "motorcycle.registered";

        public RabbitMQService(IConfiguration configuration, IServiceScopeFactory scopeFactory, ILogger<RabbitMQService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var connectionString = configuration.GetConnectionString("RabbitMQ") ?? "amqp://localhost";

            try
            {
                var factory = new ConnectionFactory();
                factory.Uri = new Uri(connectionString);

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                SetupRabbitMQ();
                StartConsumer();

                _logger.LogInformation("RabbitMQ connection established successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish RabbitMQ connection");
                throw;
            }
        }

        private void SetupRabbitMQ()
        {
            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            // Declare queue
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Bind queue to exchange
            _channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingKey
            );

            _logger.LogInformation("RabbitMQ setup completed - Exchange: {Exchange}, Queue: {Queue}", ExchangeName, QueueName);
        }

        public async Task PublishMotorcycleRegisteredAsync(Motorcycle motorcycle)
        {
            _logger.LogInformation("Publishing motorcycle registered event for ID: {MotorcycleId}", motorcycle.Id);

            var motorcycleEvent = new MotorcycleRegisteredEventDto(
                motorcycle.Id,
                motorcycle.Year,
                motorcycle.Model,
                motorcycle.LicensePlate,
                DateTime.UtcNow
            );

            var message = JsonSerializer.Serialize(motorcycleEvent);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("Motorcycle registered event published successfully for ID: {MotorcycleId}", motorcycle.Id);
            await Task.CompletedTask;
        }

        private void StartConsumer()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received message: {Message}", message);

                    var motorcycleEvent = JsonSerializer.Deserialize<MotorcycleRegisteredEventDto>(message);

                    if (motorcycleEvent != null && motorcycleEvent.Year == 2024)
                    {
                        await ProcessMotorcycle2024NotificationAsync(motorcycleEvent);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Message processed and acknowledged");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );

            _logger.LogInformation("Started consuming messages from queue: {Queue}", QueueName);
        }

        private async Task ProcessMotorcycle2024NotificationAsync(MotorcycleRegisteredEventDto motorcycleEvent)
        {
            _logger.LogInformation("Processing 2024 motorcycle notification for ID: {MotorcycleId}", motorcycleEvent.Id);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                var notification = new NotificationLog
                {
                    MotorcycleId = motorcycleEvent.Id,
                    Message = $"2024 Motorcycle registered: {motorcycleEvent.Model} - {motorcycleEvent.LicensePlate}"
                };

                context.NotificationLogs.Add(notification);
                await context.SaveChangesAsync();

                _logger.LogInformation("2024 motorcycle notification saved to database for ID: {MotorcycleId}", motorcycleEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving 2024 motorcycle notification for ID: {MotorcycleId}", motorcycleEvent.Id);
                throw;
            }
        }
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}