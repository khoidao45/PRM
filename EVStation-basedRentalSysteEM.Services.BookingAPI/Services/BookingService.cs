using EVStation_basedRentalSystem.Services.BookingAPI.Data;
using EVStation_basedRentalSystem.Services.BookingAPI.Models;
using EVStation_basedRentalSystem.Services.BookingAPI.Models.DTO;
using EVStation_basedRentalSystem.Services.BookingAPI.Models.Dto;
using EVStation_basedRentalSystem.Services.BookingAPI.Services.IService;
using Microsoft.EntityFrameworkCore;
using EVStation_basedRentalSysteEM.Services.BookingAPI.Services.IService;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _context;
        private readonly IHopDongService _hopDongService;
        private readonly IUserService _userService;
        private readonly ICarService _carService;

        public BookingService(
            BookingDbContext context,
            IHopDongService hopDongService,
            IUserService userService,
            ICarService carService)
        {
            _context = context;
            _hopDongService = hopDongService;
            _userService = userService;
            _carService = carService;
        }

        
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync() =>
            await _context.Bookings.ToListAsync();

        // ----------------------------
        // 2️⃣ Get booking by Id
        // ----------------------------
        public async Task<Booking?> GetBookingByIdAsync(int id) =>
            await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

        // ----------------------------
        // 3️⃣ Create booking + HopDong
        // ----------------------------
        public async Task<Booking> CreateBookingAsync(BookingDTO bookingDto)
        {
            var user = await _userService.GetUserByIdAsync(bookingDto.UserId)
                ?? throw new Exception("User not found");

            var car = await _carService.GetCarByIdAsync(bookingDto.CarId)
                ?? throw new Exception("Car not found");

            double hours = (bookingDto.EndDate - bookingDto.StartDate).TotalHours;
            if (hours <= 0) throw new Exception("EndDate must be after StartDate");

            decimal totalPrice = Math.Round((decimal)hours * car.HourlyRate, 2);

            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                CarId = bookingDto.CarId,
                StationId = car.StationId,
                HopDongId = "0", // bỏ hợp đồng
                StartTime = bookingDto.StartDate,
                EndTime = bookingDto.EndDate,
                TotalPrice = totalPrice,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }


        // ----------------------------
        // 4️⃣ Confirm booking HopDong
        // ----------------------------
        

        // ----------------------------
        // 5️⃣ Update booking status (Pending → Confirmed/Cancelled/Completed)
        // ----------------------------
        public async Task<Booking?> UpdateBookingStatusAsync(int id, string newStatus)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return null;

            // Optional: validate allowed status transitions
            var allowed = new List<string> { "Pending", "Confirmed", "Cancelled", "Completed" };
            if (!allowed.Contains(newStatus))
                throw new Exception($"Invalid status: {newStatus}");

            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return booking;
        }

        // ----------------------------
        // 6️⃣ Cancel booking
        // ----------------------------
        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return false;

            booking.Status = "Cancelled"; // hủy thay vì remove để lịch sử
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
    }
    }
