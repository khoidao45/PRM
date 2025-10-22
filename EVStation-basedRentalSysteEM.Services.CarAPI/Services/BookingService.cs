using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.CarAPI.Models.DTO;
using EVStation_basedRentalSystem.Services.CarAPI.Services.IService;

namespace EVStation_basedRentalSystem.Services.CarAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int bookingId)
        {
            var response = await _httpClient.GetAsync($"/api/bookings/{bookingId}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<BookingDto>();
            return null;
        }

        public async Task<List<BookingDto>> GetBookingsByRenterIdAsync(int renterId)
        {
            return await _httpClient.GetFromJsonAsync<List<BookingDto>>($"/api/bookings/renter/{renterId}")
                   ?? new List<BookingDto>();
        }

        public async Task<List<BookingDto>> GetBookingsByUserIdAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<List<BookingDto>>($"/api/bookings/user/{userId}")
                   ?? new List<BookingDto>();
        }
    }
}
