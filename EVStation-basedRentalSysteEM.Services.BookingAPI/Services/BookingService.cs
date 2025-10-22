using EVStation_basedRentalSysteEM.Services.BookingAPI.Services.IService;
using EVStation_basedRentalSysteEM.Services.BookingAPI.utils.enums;
using EVStation_basedRentalSystem.Services.BookingAPI.Data;
using EVStation_basedRentalSystem.Services.BookingAPI.Models;
using EVStation_basedRentalSystem.Services.BookingAPI.Models.DTO;
using EVStation_basedRentalSystem.Services.BookingAPI.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _context;
        private readonly IUserService _userService;
        private readonly ICarService _carService;
        private readonly IHopDongService _hopDongService;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            BookingDbContext context,
            IUserService userService,
            ICarService carService,
            IHopDongService hopDongService,
            ILogger<BookingService> logger)
        {
            _context = context;
            _userService = userService;
            _carService = carService;
            _hopDongService = hopDongService;
            _logger = logger;
        }

        // -------------------- BASIC CRUD --------------------
        public async Task<Booking> CreateBookingAsync(BookingDTO bookingDto)
        {
            // 1️⃣ Validate user
            var user = await _userService.GetUserByIdAsync(bookingDto.UserId);
            if (user == null)
                throw new Exception("User not found");

            // 2️⃣ Validate car
            var car = await _carService.GetCarByIdAsync(bookingDto.CarId);
            if (car == null)
                throw new Exception("Car not found");

            // 3️⃣ Validate thời gian
            if (bookingDto.EndDate <= bookingDto.StartDate)
                throw new Exception("EndDate must be after StartDate");

            double hours = (bookingDto.EndDate - bookingDto.StartDate).TotalHours;
            if (hours < 1)
                hours = 1;

            // 4️⃣ Kiểm tra trùng lịch xe (Confirmed hoặc CheckIn)
            bool hasConflict = await _context.Bookings.AnyAsync(b =>
                b.CarId == bookingDto.CarId &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckIn) &&
                (
                    (bookingDto.StartDate >= b.StartTime && bookingDto.StartDate < b.EndTime) ||
                    (bookingDto.EndDate > b.StartTime && bookingDto.EndDate <= b.EndTime) ||
                    (bookingDto.StartDate <= b.StartTime && bookingDto.EndDate >= b.EndTime)
                )
            );

            if (hasConflict)
                throw new Exception("This car is already booked for the selected time range");

            // 5️⃣ Tính tổng tiền
            decimal totalPrice = Math.Round((decimal)hours * car.HourlyRate, 2);

            // 6️⃣ Tạo Booking (chưa block xe)
            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                CarId = bookingDto.CarId,
                StationId = car.StationId,
                HopDongId = null,
                StartTime = bookingDto.StartDate,
                EndTime = bookingDto.EndDate,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending, // Pending cho đến khi thanh toán xong
                CreatedAt = DateTime.UtcNow
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"📘 Booking created (Pending): ID={booking.Id}, User={user.UserId}, Car={car.Id}, Price={booking.TotalPrice}");

            return booking;
        }



        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task<Booking?> UpdateBookingStatusAsync(int id, string newStatus)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return null;

            if (!Enum.TryParse<BookingStatus>(newStatus, true, out var status))
                throw new ArgumentException("Invalid booking status");

            var oldStatus = booking.Status;
            booking.Status = status;
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 🚗 Chỉ block/unblock xe khi trạng thái thay đổi
            try
            {
                if ((status == BookingStatus.Confirmed || status == BookingStatus.CheckIn) &&
                    (oldStatus == BookingStatus.Pending || oldStatus != status))
                {
                    await _carService.BlockCarAsync(booking.Id, booking.CarId, booking.StartTime, booking.EndTime);
                    _logger.LogInformation($"✅ Car {booking.CarId} blocked (Booking {booking.Id})");
                }
                else if (status == BookingStatus.Completed || status == BookingStatus.Cancelled)
                {
                    await _carService.UnblockCarAsync(booking.Id);
                    _logger.LogInformation($"🚗 Car {booking.CarId} unblocked (Booking {booking.Id})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to update car status for booking {booking.Id}");
            }

            return booking;
        }
        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;
            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------- GET BY USER / CAR / STATION --------------------
        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
            => await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAndStatusAsync(string userId, string status)
        {
            if (!Enum.TryParse<BookingStatus>(status, true, out var s)) throw new ArgumentException("Invalid status");
            return await _context.Bookings.Where(b => b.UserId == userId && b.Status == s).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByCarIdAsync(int carId)
            => await _context.Bookings.Where(b => b.CarId == carId).ToListAsync();

        public async Task<IEnumerable<Booking>> GetBookingsByCarIdAndStatusAsync(int carId, string status)
        {
            if (!Enum.TryParse<BookingStatus>(status, true, out var s)) throw new ArgumentException("Invalid status");
            return await _context.Bookings.Where(b => b.CarId == carId && b.Status == s).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStationIdAsync(int stationId)
            => await _context.Bookings.Where(b => b.StationId == stationId).ToListAsync();

        public async Task<IEnumerable<Booking>> GetBookingsByStationIdAndStatusAsync(int stationId, string status)
        {
            if (!Enum.TryParse<BookingStatus>(status, true, out var s)) throw new ArgumentException("Invalid status");
            return await _context.Bookings.Where(b => b.StationId == stationId && b.Status == s).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime start, DateTime end)
            => await _context.Bookings.Where(b => b.StartTime >= start && b.EndTime <= end).ToListAsync();

        public async Task<IEnumerable<Booking>> GetConfirmedBookingsAsync()
            => await _context.Bookings.Where(b => b.Status == BookingStatus.Confirmed).ToListAsync();

        public async Task<IEnumerable<Booking>> GetActiveBookingsAsync()
            => await _context.Bookings.Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckIn).ToListAsync();

        // -------------------- AGGREGATE / STATS --------------------
        public async Task<int> CountBookingsByUserAsync(string userId)
            => await _context.Bookings.CountAsync(b => b.UserId == userId);

        public async Task<int> CountBookingsByCarAsync(int carId)
            => await _context.Bookings.CountAsync(b => b.CarId == carId);

        public async Task<decimal> SumRevenueByDateRangeAsync(DateTime start, DateTime end)
            => await _context.Bookings
                .Where(b => b.StartTime >= start && b.EndTime <= end && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed))
                .SumAsync(b => b.TotalPrice);

        // -------------------- BOOKING DETAIL --------------------
        public async Task<BookingDetailDTO?> GetBookingDetailByIdAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return null;

            var user = await _userService.GetUserByIdAsync(booking.UserId);
            var car = await _carService.GetCarByIdAsync(booking.CarId);

            return new BookingDetailDTO
            {
                BookingId = booking.Id,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                HopDongId = booking.HopDongId,

                UserId = booking.UserId,
                UserName = user?.FullName,
                UserEmail = user?.Email,
                UserPhone = user?.PhoneNumber,

                CarId = booking.CarId,
                CarBrand = car?.Brand,
                CarModel = car?.Model,
                LicensePlate = car?.LicensePlate,
                CarColor = car?.Color,

                StationId = booking.StationId,
                StationName = "", // call StationService nếu có
                StationAddress = "",
                StationCity = "",

                ContractNumber = booking.HopDongId,
                ContractSignedDate = null,
                ContractStatus = null,

                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }

        public async Task<IEnumerable<BookingDetailDTO>> GetAllBookingDetailsAsync()
        {
            var bookings = await _context.Bookings.ToListAsync();
            var result = new List<BookingDetailDTO>();
            foreach (var b in bookings)
            {
                var detail = await GetBookingDetailByIdAsync(b.Id);
                if (detail != null) result.Add(detail);
            }
            return result;
        }

        // -------------------- STAFF-SPECIFIC --------------------
        public async Task<IEnumerable<BookingDetailDTO>> GetBookingsForStaffAsync(string? userId = null, string? status = null,
            int? carId = null, int? stationId = null, DateTime? start = null, DateTime? end = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (!string.IsNullOrEmpty(userId)) query = query.Where(b => b.UserId == userId);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, true, out var s)) query = query.Where(b => b.Status == s);
            if (carId.HasValue) query = query.Where(b => b.CarId == carId.Value);
            if (stationId.HasValue) query = query.Where(b => b.StationId == stationId.Value);
            if (start.HasValue) query = query.Where(b => b.StartTime >= start.Value);
            if (end.HasValue) query = query.Where(b => b.EndTime <= end.Value);

            var list = await query.ToListAsync();
            var result = new List<BookingDetailDTO>();
            foreach (var b in list)
            {
                var detail = await GetBookingDetailByIdAsync(b.Id);
                if (detail != null) result.Add(detail);
            }
            return result;
        }

        public async Task<BookingDetailDTO?> UpdateBookingStatusByStaffAsync(int bookingId, string newStatus)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return null;

            if (!Enum.TryParse<BookingStatus>(newStatus, true, out var s))
                throw new ArgumentException("Invalid status");

            booking.Status = s;
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetBookingDetailByIdAsync(bookingId);
        }

        public async Task<IEnumerable<BookingDetailDTO>> GetBookingsRequiringCheckInAsync()
        {
            var bookings = await _context.Bookings.Where(b => b.Status == BookingStatus.Confirmed).ToListAsync();
            var result = new List<BookingDetailDTO>();
            foreach (var b in bookings)
            {
                var detail = await GetBookingDetailByIdAsync(b.Id);
                if (detail != null) result.Add(detail);
            }
            return result;
        }

        // -------------------- ADMIN / REPORTING --------------------
        public async Task<IEnumerable<BookingDetailDTO>> GetBookingsByFilterAsync(string? userId = null, string? carId = null,
            string? stationId = null, string? status = null, DateTime? start = null, DateTime? end = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (!string.IsNullOrEmpty(userId)) query = query.Where(b => b.UserId == userId);
            if (!string.IsNullOrEmpty(carId) && int.TryParse(carId, out var carIdInt)) query = query.Where(b => b.CarId == carIdInt);
            if (!string.IsNullOrEmpty(stationId) && int.TryParse(stationId, out var stationIdInt)) query = query.Where(b => b.StationId == stationIdInt);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, true, out var s)) query = query.Where(b => b.Status == s);
            if (start.HasValue) query = query.Where(b => b.StartTime >= start.Value);
            if (end.HasValue) query = query.Where(b => b.EndTime <= end.Value);

            var list = await query.ToListAsync();
            var result = new List<BookingDetailDTO>();
            foreach (var b in list)
            {
                var detail = await GetBookingDetailByIdAsync(b.Id);
                if (detail != null) result.Add(detail);
            }
            return result;
        }

        public async Task<IEnumerable<BookingHistoryDTO>> GetBookingHistoryAsync(int bookingId)
        {
            // TODO: implement with BookingStatusHistory table if available
            return new List<BookingHistoryDTO>();
        }

        public async Task<PaginatedList<BookingDetailDTO>> GetPaginatedBookingsAsync(int pageIndex = 1, int pageSize = 20,
            string? status = null, string? userId = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (!string.IsNullOrEmpty(userId)) query = query.Where(b => b.UserId == userId);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, true, out var s)) query = query.Where(b => b.Status == s);

            var count = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var list = new List<BookingDetailDTO>();
            foreach (var b in items)
            {
                var detail = await GetBookingDetailByIdAsync(b.Id);
                if (detail != null) list.Add(detail);
            }

            return new PaginatedList<BookingDetailDTO>
            {
                Items = list,
                PageIndex = pageIndex,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }

        public async Task<decimal> CalculateTotalRevenueAsync(DateTime start, DateTime end, string? stationId = null)
        {
            var query = _context.Bookings.AsQueryable();
            query = query.Where(b => b.StartTime >= start && b.EndTime <= end && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed));
            if (!string.IsNullOrEmpty(stationId) && int.TryParse(stationId, out var sId))
                query = query.Where(b => b.StationId == sId);

            return await query.SumAsync(b => b.TotalPrice);
        }

        public async Task<int> CountActiveBookingsAsync()
            => await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckIn);
    }
}
