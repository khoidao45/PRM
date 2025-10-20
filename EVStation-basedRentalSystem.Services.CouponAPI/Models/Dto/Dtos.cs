using System.Text.Json.Serialization;
using EVStation_basedRentalSystem.Services.AuthAPI.Models;

// ======================
// ApplicationUser DTO
// ======================
public class ApplicationUserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProfileImageUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RenterProfileDto? RenterProfile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StaffProfileDto? StaffProfile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AdminProfileDto? AdminProfile { get; set; }
}

// ======================
// AdminProfile DTO
// ======================
public class AdminProfileDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string RoleLevel { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ManagedStation { get; set; }

    public bool CanApproveUsers { get; set; }
    public bool CanManageStaff { get; set; }
    public bool CanViewReports { get; set; }
}

// ======================
// StaffProfile DTO
// ======================
public class StaffProfileDto
{
    public string Id { get; set; }
    public string FullName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Position { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Department { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? WorkShift { get; set; }
}

// ======================
// RenterProfile DTO
// ======================
public class RenterProfileDto
{
    public string Id { get; set; }
    public string FullName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PhoneNumber { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DriverLicenseNumber { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IdentityCardNumber { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Gender { get; set; }
}

// ======================
// Extension Method: Map to DTO
// ======================
public static class ApplicationUserExtensions
{
    public static ApplicationUserDto ToDto(this ApplicationUser user)
    {
        return new ApplicationUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            ProfileImageUrl = user.ProfileImageUrl,
            RenterProfile = user.Role == "Renter" ? user.RenterProfile?.ToDto() : null,
            StaffProfile = user.Role == "Staff" ? user.StaffProfile?.ToDto() : null,
            AdminProfile = user.Role == "Admin" ? user.AdminProfile?.ToDto() : null
        };
    }

    public static RenterProfileDto ToDto(this RenterProfile profile)
    {
        return new RenterProfileDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            PhoneNumber = profile.PhoneNumber,
            Address = profile.Address,
            DriverLicenseNumber = profile.DriverLicenseNumber,
            IdentityCardNumber = profile.IdentityCardNumber,
            Gender = profile.Gender
        };
    }

    public static StaffProfileDto ToDto(this StaffProfile profile)
    {
        return new StaffProfileDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Position = profile.Position,
            Department = profile.Department,
            WorkShift = profile.WorkShift
        };
    }

    public static AdminProfileDto ToDto(this AdminProfile profile)
    {
        return new AdminProfileDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            RoleLevel = profile.RoleLevel,
            ManagedStation = profile.ManagedStation,
            CanApproveUsers = profile.CanApproveUsers,
            CanManageStaff = profile.CanManageStaff,
            CanViewReports = profile.CanViewReports
        };
    }
}
