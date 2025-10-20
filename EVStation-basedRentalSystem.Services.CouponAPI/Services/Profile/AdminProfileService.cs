using EVStation_basedRentalSystem.Services.AuthAPI.Data;
using EVStation_basedRentalSystem.Services.AuthAPI.Models;
using EVStation_basedRentalSystem.Services.AuthAPI.Services.IService.Profile;
using Microsoft.EntityFrameworkCore;

namespace EVStation_basedRentalSystem.Services.AuthAPI.Services.Profile
{
    public class AdminProfileService : IAdminProfileService
    {
        private readonly AppDbContext _context;

        public AdminProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminProfile>> GetAllAsync()
        {
            return await _context.AdminProfiles
                .Include(a => a.User) // Include linked user
                .ToListAsync();
        }

        public async Task<AdminProfile?> GetByIdAsync(string id)
        {
            return await _context.AdminProfiles
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AdminProfile> CreateAsync(AdminProfile profile)
        {
            _context.AdminProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<AdminProfile?> UpdateAsync(AdminProfile profile)
        {
            var existing = await _context.AdminProfiles.FirstOrDefaultAsync(a => a.Id == profile.Id);
            if (existing == null) return null;

            existing.FullName = profile.FullName;
            existing.RoleLevel = profile.RoleLevel;
            existing.ManagedStation = profile.ManagedStation;
            existing.ContactNumber = profile.ContactNumber;
            existing.CanApproveUsers = profile.CanApproveUsers;
            existing.CanManageStaff = profile.CanManageStaff;
            existing.CanViewReports = profile.CanViewReports;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.AdminProfiles.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var admin = await _context.AdminProfiles.FirstOrDefaultAsync(a => a.Id == id);
            if (admin == null) return false;

            _context.AdminProfiles.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------------------
        // Role-Specific Actions
        // -------------------------------
        public async Task<bool> ApproveUserAsync(string adminId, string userId)
        {
            var admin = await _context.AdminProfiles.FirstOrDefaultAsync(a => a.Id == adminId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (admin == null || user == null) return false;

            user.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignStationAsync(string adminId, string stationName)
        {
            var admin = await _context.AdminProfiles.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin == null) return false;

            admin.ManagedStation = stationName;
            _context.AdminProfiles.Update(admin);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
