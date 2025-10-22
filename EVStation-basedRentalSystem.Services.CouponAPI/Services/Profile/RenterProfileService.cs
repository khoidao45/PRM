using EVStation_basedRentalSystem.Services.AuthAPI.Data;
using EVStation_basedRentalSystem.Services.AuthAPI.Models;
using EVStation_basedRentalSystem.Services.UserAPI.Services.IService;
using EVStation_basedRentalSystem.Services.AuthAPI.utils.enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EVStation_basedRentalSystem.Services.UserAPI.Services.Profile
{
    public class RenterProfileService : IRenterProfileService
    {
        private readonly AppDbContext _context;

        public RenterProfileService(AppDbContext context)
        {
            _context = context;
        }

        // ---------------- CRUD ----------------
        public async Task<IEnumerable<RenterProfile>> GetAllAsync()
        {
            return await _context.RenterProfiles
                .Include(r => r.User) // Eager load User
                .ToListAsync();
        }
        public async Task<RenterProfile?> CreateOrUpdateMyProfileAsync(ClaimsPrincipal userClaims, MyRenterProfileDto dto)
        {
            // Lấy userId từ token JWT
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return null;

            // Kiểm tra profile đã tồn tại chưa
            var profile = await _context.RenterProfiles.FirstOrDefaultAsync(r => r.UserId == userId);
            if (profile == null)
            {
                // Tạo mới
                profile = new RenterProfile
                {
                    UserId = userId,
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth,
                    DriverLicenseNumber = dto.DriverLicenseNumber,
                    DriverLicenseImageUrl = dto.DriverLicenseImageUrl,
                    DriverLicenseExpiry = dto.DriverLicenseExpiry,
                    DriverLicenseClass = dto.DriverLicenseClass,
                    IdentityCardNumber = dto.IdentityCardNumber,
                    IdentityCardImageUrl = dto.IdentityCardImageUrl,
                    IdentityCardIssuedDate = dto.IdentityCardIssuedDate,
                    IdentityCardIssuedPlace = dto.IdentityCardIssuedPlace,
                    LicenseStatus = 0, // Pending
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.RenterProfiles.Add(profile);
            }
            else
            {
                // Update
                profile.FullName = dto.FullName ?? profile.FullName;
                profile.PhoneNumber = dto.PhoneNumber ?? profile.PhoneNumber;
                profile.Address = dto.Address ?? profile.Address;
                profile.Gender = dto.Gender ?? profile.Gender;
                profile.DateOfBirth = dto.DateOfBirth ?? profile.DateOfBirth;

                profile.DriverLicenseNumber = dto.DriverLicenseNumber ?? profile.DriverLicenseNumber;
                profile.DriverLicenseImageUrl = dto.DriverLicenseImageUrl ?? profile.DriverLicenseImageUrl;
                profile.DriverLicenseExpiry = dto.DriverLicenseExpiry ?? profile.DriverLicenseExpiry;
                profile.DriverLicenseClass = dto.DriverLicenseClass ?? profile.DriverLicenseClass;

                profile.IdentityCardNumber = dto.IdentityCardNumber ?? profile.IdentityCardNumber;
                profile.IdentityCardImageUrl = dto.IdentityCardImageUrl ?? profile.IdentityCardImageUrl;
                profile.IdentityCardIssuedDate = dto.IdentityCardIssuedDate ?? profile.IdentityCardIssuedDate;
                profile.IdentityCardIssuedPlace = dto.IdentityCardIssuedPlace ?? profile.IdentityCardIssuedPlace;

                profile.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return profile;
        }


        public async Task<RenterProfile?> GetByIdAsync(string id)
        {
            return await _context.RenterProfiles
                .Include(r => r.User) // Eager load User
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RenterProfile> CreateAsync(RenterProfile profile)
        {
            profile.CreatedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;
            _context.RenterProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<RenterProfile?> UpdateAsync(RenterProfile profile)
        {
            var existing = await _context.RenterProfiles.FirstOrDefaultAsync(r => r.Id == profile.Id);
            if (existing == null) return null;

            existing.FullName = profile.FullName ?? existing.FullName;
            existing.PhoneNumber = profile.PhoneNumber ?? existing.PhoneNumber;
            existing.Address = profile.Address ?? existing.Address;
            existing.Gender = profile.Gender ?? existing.Gender;
            existing.DateOfBirth = profile.DateOfBirth ?? existing.DateOfBirth;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.RenterProfiles.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var renter = await _context.RenterProfiles.FindAsync(id);
            if (renter == null) return false;
            _context.RenterProfiles.Remove(renter);
            await _context.SaveChangesAsync();
            return true;
        }

        // ---------------- RENTER actions ----------------
        public async Task<RenterProfile?> UpdateDriverLicenseAsync(
            string renterId, string licenseNumber, string imageUrl,
            DateTime? expiryDate = null, string? licenseClass = null)
        {
            var renter = await _context.RenterProfiles.FirstOrDefaultAsync(r => r.Id == renterId);
            if (renter == null) return null;

            renter.DriverLicenseNumber = licenseNumber;
            renter.DriverLicenseImageUrl = imageUrl;
            renter.DriverLicenseExpiry = expiryDate;
            renter.DriverLicenseClass = licenseClass;
            renter.LicenseStatus = LicenseVerificationStatus.Pending;
            renter.UpdatedAt = DateTime.UtcNow;

            _context.RenterProfiles.Update(renter);
            await _context.SaveChangesAsync();
            return renter;
        }

        public async Task<RenterProfile?> UpdateIdentityCardAsync(
            string renterId, string cardNumber, string imageUrl,
            DateTime? issuedDate = null, string? issuedPlace = null)
        {
            var renter = await _context.RenterProfiles.FirstOrDefaultAsync(r => r.Id == renterId);
            if (renter == null) return null;

            renter.IdentityCardNumber = cardNumber;
            renter.IdentityCardImageUrl = imageUrl;
            renter.IdentityCardIssuedDate = issuedDate;
            renter.IdentityCardIssuedPlace = issuedPlace;
            renter.UpdatedAt = DateTime.UtcNow;

            _context.RenterProfiles.Update(renter);
            await _context.SaveChangesAsync();
            return renter;
        }

        // ---------------- STAFF action ----------------
        public async Task<RenterProfile?> ApproveRenterProfileAsync(string renterId)
        {
            var renter = await _context.RenterProfiles.FirstOrDefaultAsync(r => r.Id == renterId);
            if (renter == null) return null;

            renter.LicenseStatus = LicenseVerificationStatus.Approved;
      
            renter.ReviewNote = "Approved by staff";
            renter.UpdatedAt = DateTime.UtcNow;

            _context.RenterProfiles.Update(renter);
            await _context.SaveChangesAsync();
            return renter;
        }

        public async Task<RenterProfile?> RejectRenterProfileAsync(string renterId, string reason)
        {
            var renter = await _context.RenterProfiles.FirstOrDefaultAsync(r => r.Id == renterId);
            if (renter == null) return null;

            renter.LicenseStatus = LicenseVerificationStatus.Rejected;
          
            renter.ReviewNote = reason;
            renter.UpdatedAt = DateTime.UtcNow;

            _context.RenterProfiles.Update(renter);
            await _context.SaveChangesAsync();
            return renter;
        }

    }
}
