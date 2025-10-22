using System.Collections.Generic;
using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.CarAPI.Models.DTO;

namespace EVStation_basedRentalSystem.Services.CarAPI.Services.IService
{
    public interface IBookingService
    {
        Task<BookingDto?> GetBookingByIdAsync(int bookingId);
        Task<List<BookingDto>> GetBookingsByRenterIdAsync(int renterId);
        Task<List<BookingDto>> GetBookingsByUserIdAsync(string userId);
    }
}
