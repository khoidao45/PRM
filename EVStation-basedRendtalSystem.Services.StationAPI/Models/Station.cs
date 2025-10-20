using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVStation_basedRendtalSystem.Services.StationAPI.utils.enums;

namespace EVStation_basedRentalSystem.Services.StationAPI.Models
{
    public class Station
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }
        [Column(TypeName = "nvarchar(30)")]
        public StationStatus Status { get; set; } = StationStatus.Active;  // "Active", "Inactive", "Maintenance"

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
