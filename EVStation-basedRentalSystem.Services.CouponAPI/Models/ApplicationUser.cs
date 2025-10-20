using Microsoft.AspNetCore.Identity;
using System;

namespace EVStation_basedRentalSystem.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Role { get; set; } = "Renter";

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // 🔥 Add these two for refresh token support
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // navigation properties
        public RenterProfile? RenterProfile { get; set; }
        public StaffProfile? StaffProfile { get; set; }
        public AdminProfile? AdminProfile { get; set; }

    }
}
