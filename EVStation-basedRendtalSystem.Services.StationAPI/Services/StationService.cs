using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.StationAPI.Data;
using EVStation_basedRentalSystem.Services.StationAPI.Models;
using EVStation_basedRentalSystem.Services.StationAPI.Services.IService;
using Microsoft.EntityFrameworkCore;
using EVStation_basedRendtalSystem.Services.StationAPI.utils.enums;

namespace EVStation_basedRentalSystem.Services.StationAPI.Services
{
    public class StationService : IStationService
    {
        private readonly StationDbContext _context;

        public StationService(StationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Station>> GetAllStationsAsync() =>
            await _context.Stations.ToListAsync();

        public async Task<Station> GetStationByIdAsync(int id) =>
            await _context.Stations.FindAsync(id);

        public async Task<Station> AddStationAsync(Station station)
        {
            _context.Stations.Add(station);
            await _context.SaveChangesAsync();
            return station;
        }

        public async Task<Station> UpdateStationAsync(Station station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
            return station;
        }

        public async Task<bool> DeleteStationAsync(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return false;

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Station>> GetActiveStationsAsync() =>
            await _context.Stations.Where(s => s.Status == StationStatus.Active).ToListAsync();

        public async Task<IEnumerable<Station>> SearchStationsAsync(string keyword) =>
            await _context.Stations
                .Where(s => s.Name.Contains(keyword) || s.City.Contains(keyword))
                .ToListAsync();

        public async Task<bool> UpdateStationStatusAsync(int stationId, string status)
        {
            var station = await _context.Stations.FindAsync(stationId);
            if (station == null) return false;

            // Convert string to enum
            if (!Enum.TryParse<StationStatus>(status, out var newStatus))
                return false; // hoặc throw exception nếu string không hợp lệ

            station.Status = newStatus;
            station.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Station>> GetStationsWithAvailableCarsAsync()
        {
            // Later, call CarAPI using HttpClient to fetch cars by station.
            return await _context.Stations.ToListAsync();
        }
    }
}
