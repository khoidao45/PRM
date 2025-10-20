using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EVStation_basedRentalSystem.Services.AuthAPI.Models
{
    public class AdminProfile
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string FullName { get; set; }
        public string RoleLevel { get; set; } // e.g., SuperAdmin, StationAdmin
        public string? ManagedStation { get; set; }
        public string? ContactNumber { get; set; }

        public bool CanApproveUsers { get; set; } = true;
        public bool CanManageStaff { get; set; } = true;
        public bool CanViewReports { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
