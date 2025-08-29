using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MotorcycleRental.Application.Interfaces.Services;

namespace MotorcycleRental.Infrastructure.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _storagePath;
        private readonly ILogger<LocalStorageService> _logger;

        public LocalStorageService(IConfiguration configuration, ILogger<LocalStorageService> logger)
        {
            _storagePath = configuration["Storage:LocalPath"] ?? "uploads";
            _logger = logger;

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            var folderPath = Path.Combine(_storagePath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = Path.Combine(folder, fileName);
            _logger.LogInformation("File saved successfully: {FilePath}", relativePath);

            return relativePath;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_storagePath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
            }
        }
    }
}