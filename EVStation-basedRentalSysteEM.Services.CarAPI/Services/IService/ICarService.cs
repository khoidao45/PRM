using System.Collections.Generic;
using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.CarAPI.Models;
using EVStation_basedRentalSystem.Services.CarAPI.Models.DTO;

namespace EVStation_basedRentalSystem.Services.CarAPI.Services.IService
{
    public interface ICarService
    {
        // Basic CRUD
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car?> GetCarByIdAsync(int id);
        Task<Car> AddCarAsync(Car car);
        Task<Car?> UpdateCarAsync(Car car);
        Task<bool> DeleteCarAsync(int id);

        // Extended operations
        Task<IEnumerable<Car>> GetCarsByStationIdAsync(int stationId);
        Task<IEnumerable<Car>> GetAvailableCarsAsync();
        Task<bool> UpdateCarStateAsync(int carId, string newState);
        Task<bool> UpdateBatteryLevelAsync(int carId, decimal newBatteryLevel);
        Task<IEnumerable<Car>> SearchCarsAsync(string keyword);
        Task<IEnumerable<Car>> GetCarsNeedingMaintenanceAsync();

        // Booking-related queries
        Task<Car?> GetCarByBookingIdAsync(int bookingId);
        Task<IEnumerable<Car>> GetCarsByUserIdAsync(string userId);
        Task<IEnumerable<Car>> GetCarsByRenterIdAsync(int renterId);

        // Management dashboard
        Task<IEnumerable<CarDto>> GetAllCarsWithStationAsync();

        Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime start, DateTime end, int? stationId = null);
        Task UnblockExpiredCarsAsync();
        Task<int?> BlockCarAsync(int carId, DateTime start, DateTime end);
   
        Task<bool> UnblockCarAsync(int blockId);
        Task<IEnumerable<CarAvailability>> GetBlockedCarsAsync(DateTime? from = null, DateTime? to = null);


    }
}
