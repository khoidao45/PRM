using EVStation_basedRentalSystem.Services.BookingAPI.Models;
using EVStation_basedRentalSystem.Services.BookingAPI.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Services.IService
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking> CreateBookingAsync(BookingDTO bookingDto);
        Task<Booking?> UpdateBookingStatusAsync(int id, string newStatus);
        Task<bool> CancelBookingAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);

    }
}
