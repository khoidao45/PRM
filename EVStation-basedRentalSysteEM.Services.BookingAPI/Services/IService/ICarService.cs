using System.Threading.Tasks;
using EVStation_basedRentalSysteEM.Services.BookingAPI.Models.Dto;
using EVStation_basedRentalSystem.Services.BookingAPI.Models.Dto;

namespace EVStation_basedRentalSysteEM.Services.BookingAPI.Services.IService
{
    public interface ICarService
    {
        Task<CarDto?> GetCarByIdAsync(int carId);
        Task<bool> BlockCarAsync(int bookingId, int carId, DateTime startTime, DateTime endTime);
        Task<bool> UnblockCarAsync(int bookingId);
        Task<bool> UpdateCarStatusAsync(int carId, string newStatus);
        Task<bool> UpdateAvailabilityAsync(int carId, bool isAvailable);
        Task<IEnumerable<CarDto>?> GetAllCarsAsync();
    }
}
