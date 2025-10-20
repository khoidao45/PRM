using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVStation_basedRentalSystem.Services.CarAPI.utils.enums;

namespace EVStation_basedRentalSystem.Services.CarAPI.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StationId { get; set; }  // FK → Station

        [Required]
        [MaxLength(20)]
        public string LicensePlate { get; set; }

        [Range(1, 20)]
        public int Seat { get; set; }

        [MaxLength(50)]
        public string Model { get; set; }

        [MaxLength(50)]
        public string Brand { get; set; }

        [Range(1900, 2100)]
        public int Year { get; set; }

        [MaxLength(30)]
        public string Color { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BatteryCapacity { get; set; }    // kWh

        [Range(0, double.MaxValue)]
        public decimal CurrentBatteryLevel { get; set; } // percentage or kWh

        [Range(0, double.MaxValue)]
        public decimal HourlyRate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DailyRate { get; set; }

        public DateTime? LastMaintenanceDay { get; set; }

        public DateTime? RegistrationExpiry { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public CarState State { get; set; } = CarState.Available;    // e.g., "Available", "In Use", "Maintenance"

        [Column(TypeName = "nvarchar(30)")]
        public CarStatus Status { get; set; } = CarStatus.Active;   // general status flag

        // --- Relationships (optional) ---
        // public Station Station { get; set; }
        // public ICollection<Booking> Bookings { get; set; }
        // public ICollection<Rating> Ratings { get; set; }
    }
}
