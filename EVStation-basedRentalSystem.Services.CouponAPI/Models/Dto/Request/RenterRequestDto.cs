
namespace EVStation_basedRentalSystem.Services.AuthAPI.Models.Dto.Request
{
    public class RenterRequestDto
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }

        public string? DriverLicenseNumber { get; set; }
        public string? DriverLicenseClass { get; set; }
        public DateTime? DriverLicenseExpiry { get; set; }
        public string? DriverLicenseImageUrl { get; set; }

        public string? IdentityCardNumber { get; set; }
        public DateTime? IdentityCardIssuedDate { get; set; }
        public string? IdentityCardIssuedPlace { get; set; }
        public string? IdentityCardImageUrl { get; set; }
    }
}