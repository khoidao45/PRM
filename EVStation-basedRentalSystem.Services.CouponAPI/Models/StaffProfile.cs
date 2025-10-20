using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EVStation_basedRentalSystem.Services.AuthAPI.Models
{
    public class StaffProfile
    {
        [Key]
        public string Id { get; set; } // SAME as ApplicationUser.Id

        [Required]
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? StationAssigned { get; set; }
        public string? Department { get; set; }
        public string? WorkShift { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
