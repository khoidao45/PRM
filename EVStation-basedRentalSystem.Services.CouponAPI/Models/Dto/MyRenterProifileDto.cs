
using EVStation_basedRentalSystem.Services.AuthAPI.Models;
public class MyRenterProfileDto
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
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
