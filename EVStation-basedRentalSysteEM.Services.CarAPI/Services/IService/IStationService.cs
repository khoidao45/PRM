using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.CarAPI.Models.DTO;

namespace EVStation_basedRentalSystem.Services.CarAPI.Services.IService
{
    public interface IStationService
    {
        Task<StationDto?> GetStationByIdAsync(int stationId);
    }
}
