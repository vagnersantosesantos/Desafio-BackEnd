using Microsoft.AspNetCore.Http;

namespace MotorcycleRental.Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task DeleteFileAsync(string filePath);
    }
}
