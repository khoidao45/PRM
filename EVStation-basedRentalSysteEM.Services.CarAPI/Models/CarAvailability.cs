using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVStation_basedRentalSystem.Services.CarAPI.Models
{
    public class CarAvailability
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Car))]
        public int CarId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string Status { get; set; } = "Available"; // or "Available"

        public Car Car { get; set; }
    }
}
