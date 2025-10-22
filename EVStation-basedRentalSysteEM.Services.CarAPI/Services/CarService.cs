using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.CarAPI.Data;
using EVStation_basedRentalSystem.Services.CarAPI.Models;
using EVStation_basedRentalSystem.Services.CarAPI.Models.DTO;
using EVStation_basedRentalSystem.Services.CarAPI.Services.IService;
using EVStation_basedRentalSystem.Services.CarAPI.utils.enums;
using Microsoft.EntityFrameworkCore;

namespace EVStation_basedRentalSystem.Services.CarAPI.Services
{
    public class CarService : ICarService
    {
        private readonly CarDbContext _context;
        private readonly IBookingService _bookingService;
        private readonly IStationService _stationService;

        public CarService(CarDbContext context, IBookingService bookingService, IStationService stationService)
        {
            _context = context;
            _bookingService = bookingService;
            _stationService = stationService;
        }

        // ------------------ Basic CRUD ------------------
        public async Task<IEnumerable<Car>> GetAllCarsAsync() => await _context.Cars.ToListAsync();

        public async Task<Car?> GetCarByIdAsync(int id) => await _context.Cars.FindAsync(id);

        public async Task<Car> AddCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<Car?> UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return false;
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return true;
        }

        // ------------------ Extended Operations ------------------
        public async Task<IEnumerable<Car>> GetCarsByStationIdAsync(int stationId)
            => await _context.Cars.Where(c => c.StationId == stationId).ToListAsync();

       
        public async Task<bool> UpdateCarStateAsync(int carId, string newState)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return false;
            if (!Enum.TryParse<CarState>(newState, true, out var state)) return false;

            car.State = state;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBatteryLevelAsync(int carId, decimal newBatteryLevel)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return false;

            car.CurrentBatteryLevel = newBatteryLevel;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Car>> SearchCarsAsync(string keyword)
            => await _context.Cars
                .Where(c => c.Brand.Contains(keyword) || c.Model.Contains(keyword) || c.Color.Contains(keyword))
                .ToListAsync();

        public async Task<IEnumerable<Car>> GetCarsNeedingMaintenanceAsync()
            => await _context.Cars.Where(c => c.State == CarState.Maintenance).ToListAsync();

        // ------------------ Booking-related ------------------
        public async Task<Car?> GetCarByBookingIdAsync(int bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null) return null;
            return await _context.Cars.FindAsync(booking.CarId);
        }

        public async Task<IEnumerable<Car>> GetCarsByUserIdAsync(string userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            var carIds = bookings.Select(b => b.CarId).Distinct();
            return await _context.Cars.Where(c => carIds.Contains(c.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetCarsByRenterIdAsync(int renterId)
        {
            var bookings = await _bookingService.GetBookingsByRenterIdAsync(renterId);
            var carIds = bookings.Select(b => b.CarId).Distinct();
            return await _context.Cars.Where(c => carIds.Contains(c.Id)).ToListAsync();
        }

        // ------------------ Combine Car + Station info ------------------
        public async Task<IEnumerable<CarDto>> GetAllCarsWithStationAsync()
        {
            var cars = await _context.Cars.ToListAsync();
            var stationIds = cars.Select(c => c.StationId).Distinct();
            var stationTasks = stationIds.Select(id => _stationService.GetStationByIdAsync(id));
            var stations = await Task.WhenAll(stationTasks);
            var stationDict = stations.Where(s => s != null).ToDictionary(s => s!.Id);

            return cars.Select(car => new CarDto
            {
                Id = car.Id,
                StationId = car.StationId,
                LicensePlate = car.LicensePlate,
                Seat = car.Seat,
                Model = car.Model,
                Brand = car.Brand,
                Year = car.Year,
                Color = car.Color,
                BatteryCapacity = car.BatteryCapacity,
                CurrentBatteryLevel = car.CurrentBatteryLevel,
                HourlyRate = car.HourlyRate,
                DailyRate = car.DailyRate,
                LastMaintenanceDay = car.LastMaintenanceDay,
                RegistrationExpiry = car.RegistrationExpiry,
                ImageUrl = car.ImageUrl,
                State = car.State,
                Status = car.Status,
                Station = stationDict.ContainsKey(car.StationId) ? stationDict[car.StationId] : null
            }).ToList();
        }
        public async Task<int?> BlockCarAsync(int carId, DateTime start, DateTime end)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null || car.State == CarState.Unavailable)
                return null;

            // update trạng thái xe
            car.State = CarState.Unavailable;

            var block = new CarAvailability
            {
                CarId = carId,
                StartTime = start,
                EndTime = end,
                Status = "Blocked"
            };

            _context.Add(block);
            await _context.SaveChangesAsync();

            return block.Id; // trả về ID block
        }
        public async Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime start, DateTime end, int? stationId = null)
        {
            // 1️⃣ Lấy tất cả xe (filter theo station nếu có)
            var allCarsQuery = _context.Cars.AsQueryable();
            if (stationId.HasValue)
                allCarsQuery = allCarsQuery.Where(c => c.StationId == stationId.Value);

            var allCars = await allCarsQuery.ToListAsync();

            // 2️⃣ Lấy tất cả block hoặc booking đã biết
            var allBookings = new List<BookingDto>();

            // Giả sử bạn có danh sách bookingId nào đó cần check
            // Ví dụ, nếu CarId = booking.CarId => bạn có thể loop từng bookingId
            // Mình để tạm ví dụ:
            foreach (var car in allCars)
            {
                // chỉ lấy booking id đã biết, ví dụ 1,2,3...
                var booking = await _bookingService.GetBookingByIdAsync(car.Id);
                if (booking != null)
                    allBookings.Add(booking);
            }

            var conflictingBookings = allBookings
                .Where(b => (b.Status == "Confirmed" || b.Status == "CheckIn") &&
                            ((start >= b.StartTime && start < b.EndTime) ||
                             (end > b.StartTime && end <= b.EndTime) ||
                             (start <= b.StartTime && end >= b.EndTime)))
                .ToList();

            var blockedCarIds = conflictingBookings.Select(b => b.CarId).Distinct().ToHashSet();

            // 3️⃣ Lọc xe khả dụng
            var availableCars = allCars.Where(c => !blockedCarIds.Contains(c.Id));

            return availableCars;
        }

        public async Task UnblockExpiredCarsAsync()
        {
            var now = DateTime.UtcNow;
            var expiredBlocks = await _context.Set<CarAvailability>()
                .Where(a => a.Status == "Blocked" && a.EndTime < now)
                .ToListAsync();

            foreach (var block in expiredBlocks)
            {
                var car = await _context.Cars.FindAsync(block.CarId);
                if (car != null && car.State == CarState.Unavailable)
                    car.State = CarState.Available;

                block.Status = "Available";
            }

            await _context.SaveChangesAsync();
        }
        public async Task<bool> UnblockCarAsync(int blockId)
        {
            var block = await _context.Set<CarAvailability>().FindAsync(blockId);
            if (block == null) return false;

            var car = await _context.Cars.FindAsync(block.CarId);
            if (car != null && car.State == CarState.Unavailable)
                car.State = CarState.Available;

            _context.CarAvailabilities.Remove(block); // ⚡ xóa hẳn record
            await _context.SaveChangesAsync();
            return true;
        }

        // Lấy danh sách tất cả các block theo thời gian
        public async Task<IEnumerable<CarAvailability>> GetBlockedCarsAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Set<CarAvailability>().AsQueryable();
            query = query.Where(a => a.Status == "Blocked");

            if (from.HasValue)
                query = query.Where(a => a.StartTime >= from.Value);
            if (to.HasValue)
                query = query.Where(a => a.EndTime <= to.Value);

            return await query.ToListAsync();
        }

        public Task<IEnumerable<Car>> GetAvailableCarsAsync()
        {
            throw new NotImplementedException();
        }

       

    }

}

    

