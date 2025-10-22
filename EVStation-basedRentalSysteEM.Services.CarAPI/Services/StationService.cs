using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.CarAPI.Models.DTO;
using EVStation_basedRentalSystem.Services.CarAPI.Services.IService;

namespace EVStation_basedRentalSystem.Services.CarAPI.Services
{
    public class StationService : IStationService
    {
        private readonly HttpClient _httpClient;

        public StationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StationDto?> GetStationByIdAsync(int stationId)
        {
            var response = await _httpClient.GetAsync($"/api/station/{stationId}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<StationDto>();
            return null;
        }
    }
}
