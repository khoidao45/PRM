using EVStation_basedRentalSysteEM.Services.BookingAPI.utils.enums;
using EVStation_basedRentalSystem.Services.BookingAPI.Models;
using EVStation_basedRentalSystem.Services.BookingAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Services.IService
{
    public interface IBookingService
    {
        // ----- Basic CRUD -----
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking> CreateBookingAsync(BookingDTO bookingDto);
        Task<Booking?> UpdateBookingStatusAsync(int id, string newStatus);
        Task<bool> CancelBookingAsync(int id);

        // ----- Get by user -----
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAndStatusAsync(string userId, string status);

        // ----- Get by car -----
        Task<IEnumerable<Booking>> GetBookingsByCarIdAsync(int carId);
        Task<IEnumerable<Booking>> GetBookingsByCarIdAndStatusAsync(int carId, string status);

        // ----- Get by station -----
        Task<IEnumerable<Booking>> GetBookingsByStationIdAsync(int stationId);
        Task<IEnumerable<Booking>> GetBookingsByStationIdAndStatusAsync(int stationId, string status);

        // ----- Get by date range -----
        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime start, DateTime end);

        // ----- Get confirmed or active bookings -----
        Task<IEnumerable<Booking>> GetConfirmedBookingsAsync();
        Task<IEnumerable<Booking>> GetActiveBookingsAsync(); // active = Confirmed & not Completed or Cancelled

        // ----- Aggregate / stats -----
        Task<int> CountBookingsByUserAsync(string userId);
        Task<int> CountBookingsByCarAsync(int carId);
        Task<decimal> SumRevenueByDateRangeAsync(DateTime start, DateTime end);

        // ----- Optional: include related data -----
        Task<BookingDetailDTO?> GetBookingDetailByIdAsync(int id); // return booking + user + car + hopdong info
        Task<IEnumerable<BookingDetailDTO>> GetAllBookingDetailsAsync();

        // ----- Staff-specific functions -----
        Task<IEnumerable<BookingDetailDTO>> GetBookingsForStaffAsync(
            string? userId = null,
            string? status = null,
            int? carId = null,
            int? stationId = null,
            DateTime? start = null,
            DateTime? end = null);

        Task<BookingDetailDTO?> UpdateBookingStatusByStaffAsync(int bookingId, string newStatus);

        Task<IEnumerable<BookingDetailDTO>> GetBookingsRequiringCheckInAsync(); // Bookings that need staff check-in

        // ----- Admin-specific / reporting -----
        Task<IEnumerable<BookingDetailDTO>> GetBookingsByFilterAsync(
            string? userId = null,
            string? carId = null,
            string? stationId = null,
            string? status = null,
            DateTime? start = null,
            DateTime? end = null);

        Task<IEnumerable<BookingHistoryDTO>> GetBookingHistoryAsync(int bookingId);

        Task<PaginatedList<BookingDetailDTO>> GetPaginatedBookingsAsync(
            int pageIndex = 1,
            int pageSize = 20,
            string? status = null,
            string? userId = null);

        Task<decimal> CalculateTotalRevenueAsync(DateTime start, DateTime end, string? stationId = null);

        Task<int> CountActiveBookingsAsync();
    }

    // ----- Optional supporting DTOs -----
    public record BookingHistoryDTO
    {
        public int BookingId { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } // Staff/Admin who changed
    }

    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
        public class BookingDetailDTO
        {
            // ----- Booking info -----
            public int BookingId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public decimal TotalPrice { get; set; }
            public BookingStatus Status { get; set; }
            public string? HopDongId { get; set; }

            // ----- User info -----
            public string UserId { get; set; }
            public string? UserName { get; set; }
            public string? UserEmail { get; set; }
            public string? UserPhone { get; set; }

            // ----- Car info -----
            public int CarId { get; set; }
            public string? CarBrand { get; set; }
            public string? CarModel { get; set; }
            public string? LicensePlate { get; set; }
            public string? CarColor { get; set; }

            // ----- Station info -----
            public int StationId { get; set; }
            public string? StationName { get; set; }
            public string? StationAddress { get; set; }
            public string? StationCity { get; set; }

            // ----- Contract / HopDong info -----
            public string? ContractNumber { get; set; }
            public DateTime? ContractSignedDate { get; set; }
            public string? ContractStatus { get; set; }

            // ----- Metadata -----
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }


