using MotorcycleRental.Application.DTOs;

namespace MotorcycleRental.Application.Interfaces.Services
{
    public interface IRentalService
    {
        Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto dto);
        Task<RentalResponseDto> GetByIdAsync(string id);
        Task<ReturnRentalTotalCoastDto> ReturnRentalAsync(string rentalId, DateTime returnDate);
    }
}
