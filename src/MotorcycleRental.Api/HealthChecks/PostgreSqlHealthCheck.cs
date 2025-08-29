using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace MotorcycleRental.Api.HealthChecks
{
    public class PostgreSqlHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public PostgreSqlHealthCheck(string connectionString, ILogger<PostgreSqlHealthCheck> logger)
        {
            _connectionString = connectionString;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                await command.ExecuteScalarAsync(cancellationToken);

                return HealthCheckResult.Healthy("PostgreSQL database is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("PostgreSQL database is unhealthy", ex);
            }
        }
    }
}