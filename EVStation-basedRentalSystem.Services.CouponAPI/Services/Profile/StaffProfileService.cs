using EVStation_basedRentalSystem.Services.AuthAPI.Data;
using EVStation_basedRentalSystem.Services.AuthAPI.Models;
using EVStation_basedRentalSystem.Services.AuthAPI.utils.enums;
using Microsoft.EntityFrameworkCore;

namespace EVStation_basedRentalSystem.Services.UserAPI.Services.Profile
{
    public class StaffProfileService : IStaffProfileService
    {
        private readonly AppDbContext _context;

        public StaffProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffProfile>> GetAllAsync()
        {
            return await _context.StaffProfiles
                .Include(s => s.User) // Include linked user
                .ToListAsync();
        }

        public async Task<StaffProfile?> GetByIdAsync(string id)
        {
            return await _context.StaffProfiles
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StaffProfile> CreateAsync(StaffProfile profile)
        {
            _context.StaffProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<StaffProfile?> UpdateAsync(StaffProfile profile)
        {
            var existing = await _context.StaffProfiles.FirstOrDefaultAsync(s => s.Id == profile.Id);
            if (existing == null) return null;

            existing.FullName = profile.FullName;
            existing.Position = profile.Position;
            existing.StationAssigned = profile.StationAssigned;
            existing.Department = profile.Department;
            existing.WorkShift = profile.WorkShift;
            existing.Email = profile.Email;
            existing.PhoneNumber = profile.PhoneNumber;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.StaffProfiles.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var staff = await _context.StaffProfiles.FirstOrDefaultAsync(s => s.Id == id);
            if (staff == null) return false;

            _context.StaffProfiles.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------------------
        // Role-Specific Actions
        // -------------------------------
        public async Task<StaffProfile?> AssignShiftAsync(string staffId, string workShift)
        {
            var staff = await _context.StaffProfiles.FirstOrDefaultAsync(s => s.Id == staffId);
            if (staff == null) return null;

            staff.WorkShift = workShift;
            staff.UpdatedAt = DateTime.UtcNow;
            _context.StaffProfiles.Update(staff);
            await _context.SaveChangesAsync();
            return staff;
        }
        public async Task<bool> ApproveRenterAsync(string renterId)  // string Id từ RenterProfile
        {
            var renter = await _context.RenterProfiles
                .FirstOrDefaultAsync(r => r.Id == renterId);

            if (renter == null) return false;

            renter.LicenseStatus = LicenseVerificationStatus.Approved;
            renter.UpdatedAt = DateTime.UtcNow;

            _context.RenterProfiles.Update(renter);
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------------------
        // Booking management
        // -------------------------------
       

        public async Task<StaffProfile?> AssignDepartmentAsync(string staffId, string department)
        {
            var staff = await _context.StaffProfiles.FirstOrDefaultAsync(s => s.Id == staffId);
            if (staff == null) return null;

            staff.Department = department;
            staff.UpdatedAt = DateTime.UtcNow;
            _context.StaffProfiles.Update(staff);
            await _context.SaveChangesAsync();
            return staff;
        }
    }
}
