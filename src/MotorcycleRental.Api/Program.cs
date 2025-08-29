using Infra.Data.Context;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MotorcycleRental.Api.HealthChecks;
using MotorcycleRental.Application.Interfaces.Repositories;
using MotorcycleRental.Application.Interfaces.Services;
using MotorcycleRental.Application.Services;
using MotorcycleRental.Infrastructure.Repositories;
using MotorcycleRental.Infrastructure.Services;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Database
builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("MotorcycleRental.Api");
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
});

// Repository Registration
builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddScoped<IDeliveryDriverRepository, DeliveryDriverRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<INotificationLogRepository, NotificationLogRepository>();

// Services
builder.Services.AddScoped<IMotorcycleService, MotorcycleService>();
builder.Services.AddScoped<IDeliveryDriverService, DeliveryDriverService>();
builder.Services.AddScoped<IRentalService, RentalService>();

// Infrastructure Services Registration
builder.Services.AddSingleton<IMessageBrokerService, RabbitMQService>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();


// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<PostgreSqlHealthCheck>(
        "postgresql",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "database", "postgresql" })
    .AddCheck<RabbitMQHealthCheck>(
        "rabbitmq",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "messaging", "rabbitmq" });

// Register Health Check dependencies
builder.Services.AddSingleton<PostgreSqlHealthCheck>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection not found");
    var logger = provider.GetRequiredService<ILogger<PostgreSqlHealthCheck>>();
    return new PostgreSqlHealthCheck(connectionString, logger);
});

builder.Services.AddSingleton<RabbitMQHealthCheck>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("RabbitMQ")
        ?? throw new InvalidOperationException("RabbitMQ connection string not found");
    var logger = provider.GetRequiredService<ILogger<RabbitMQHealthCheck>>();
    return new RabbitMQHealthCheck(connectionString, logger);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Motorcycle Rental API",
        Version = "v1",
        Description = "API for managing motorcycle rentals and delivery drivers"
    });
});

// Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Motorcycle Rental API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// Health Checks endpoint
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/db", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("database")
});
app.MapHealthChecks("/health/messaging", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("messaging")
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created and migrations applied
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    try
    {
        context.Database.Migrate();
        app.Logger.LogInformation("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}

app.Run();
