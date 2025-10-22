using System.Security.Claims;
using EVStation_basedRentalSystem.Services.AuthAPI.Models;

namespace EVStation_basedRentalSystem.Services.UserAPI.Services.IService
{
    public interface IRenterProfileService
    {
        Task<IEnumerable<RenterProfile>> GetAllAsync();
        Task<RenterProfile?> GetByIdAsync(string id);
        Task<RenterProfile> CreateAsync(RenterProfile profile);
        Task<RenterProfile?> UpdateAsync(RenterProfile profile);
        Task<bool> DeleteAsync(string id);
        Task<RenterProfile?> UpdateDriverLicenseAsync(
    string renterId,
    string licenseNumber,
    string imageUrl,
    DateTime? expiryDate,
    string? licenseClass
);
        Task<RenterProfile?> UpdateIdentityCardAsync(
    string renterId,
    string cardNumber,
    string imageUrl,
    DateTime? issuedDate,
    string? issuedPlace
);
        Task<RenterProfile?> CreateOrUpdateMyProfileAsync(ClaimsPrincipal userClaims, MyRenterProfileDto dto);

        // Renter actions
        Task<RenterProfile?> ApproveRenterProfileAsync(string renterId);
        Task<RenterProfile?> RejectRenterProfileAsync(string renterId, string reason);

    }
}
