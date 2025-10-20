using EVStation_basedRentalSystem.Services.AuthAPI.Models;

namespace EVStation_basedRentalSystem.Services.AuthAPI.Services.IService.Profile
{
    public interface IAdminProfileService
    {
        Task<IEnumerable<AdminProfile>> GetAllAsync();
        Task<AdminProfile?> GetByIdAsync(string id);
        Task<AdminProfile> CreateAsync(AdminProfile profile);
        Task<AdminProfile?> UpdateAsync(AdminProfile profile);
        Task<bool> DeleteAsync(string id);

        // Role-specific actions
        Task<bool> ApproveUserAsync(string adminId, string userId);
        Task<bool> AssignStationAsync(string adminId, string stationName);
    }
}
