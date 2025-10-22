using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.BookingAPI.Models.Dto;
using EVStation_basedRentalSysteEM.Services.BookingAPI.Services.IService;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Services
{
    public class CarService : ICarService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CarService> _logger;

        public CarService(HttpClient httpClient, ILogger<CarService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // ✅ Lấy danh sách tất cả xe
        public async Task<IEnumerable<CarDto>?> GetAllCarsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<CarDto>>("api/Car");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all cars");
                return null;
            }
        }

        // ✅ Lấy thông tin xe theo ID
        public async Task<CarDto?> GetCarByIdAsync(int carId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<CarDto>($"api/Car/{carId}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching car with ID {CarId}", carId);
                return null;
            }
        }

        // ✅ Chặn xe trong thời gian có booking (Block)
        public async Task<bool> BlockCarAsync(int bookingId, int carId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var payload = new
                {
                    BookingId = bookingId,
                    CarId = carId,
                    StartTime = startTime,
                    EndTime = endTime
                };
                var response = await _httpClient.PostAsJsonAsync("api/Car/block", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking car {CarId} for booking {BookingId}", carId, bookingId);
                return false;
            }
        }


        public async Task<bool> UnblockCarAsync(int bookingId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Car/unblock/{bookingId}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking car for booking {BookingId}", bookingId);
                return false;
            }
        }


        // ✅ Cập nhật trạng thái (ví dụ: Available, Booked, Maintenance...)
        public async Task<bool> UpdateCarStatusAsync(int carId, string newStatus)
        {
            try
            {
                var payload = new { Status = newStatus };
                var response = await _httpClient.PutAsJsonAsync($"api/Car/{carId}/status", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for car {CarId}", carId);
                return false;
            }
        }

        // ✅ Cập nhật tình trạng sẵn sàng (Available/Unavailable)
        public async Task<bool> UpdateAvailabilityAsync(int carId, bool isAvailable)
        {
            try
            {
                var payload = new { IsAvailable = isAvailable };
                var response = await _httpClient.PutAsJsonAsync($"api/Car/{carId}/availability", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability for car {CarId}", carId);
                return false;
            }
        }
    }
}
