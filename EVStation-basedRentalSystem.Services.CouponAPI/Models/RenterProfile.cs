using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EVStation_basedRentalSystem.Services.AuthAPI.utils.enums;

namespace EVStation_basedRentalSystem.Services.AuthAPI.Models
{
    public class RenterProfile
    {
        [Key]
        public string Id { get; set; }  // SAME as ApplicationUser.Id

        [Required]
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public string? DriverLicenseNumber { get; set; }
        public DateTime? DriverLicenseExpiry { get; set; }
        public string? DriverLicenseClass { get; set; }
        public string? DriverLicenseImageUrl { get; set; }
        public string? IdentityCardNumber { get; set; }
        public string? IdentityCardImageUrl { get; set; }
        public DateTime? IdentityCardIssuedDate { get; set; }
        public string? IdentityCardIssuedPlace { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public LicenseVerificationStatus LicenseStatus { get; set; } = LicenseVerificationStatus.Pending;
        public string? ReviewNote { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // 🔹 Make navigation property nullable so EF won't try to insert
        [JsonIgnore]
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public string UserId { get; set; } // link to existing user
    }
}
