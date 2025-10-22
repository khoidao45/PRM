using EVStation_basedRendtalSystem.Services.PaymentAPI.Models.Dto;
using EVStation_basedRendtalSystem.Services.PaymentAPI.Services.IService;

public class BookingService : IBookingService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public BookingService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _baseUrl = config["BookingApi:BaseUrl"] ?? "https://localhost:7226/api/booking";
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int bookingId)
    {
        return await _httpClient.GetFromJsonAsync<BookingDto>($"{_baseUrl}/{bookingId}");
    }

    public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
    {
        var response = await _httpClient.PutAsync($"{_baseUrl}/{bookingId}/status?status={status}", null);
        return response.IsSuccessStatusCode;
    }
}
