using System;
using System.ComponentModel.DataAnnotations;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }   // Primary key as auto-increment integer

        [Required]
        public string UserId { get; set; }  // match AuthAPI

        [Required]
        public int CarId { get; set; }

        [Required]
        public int StationId { get; set; }

        [Required]
        public string HopDongId { get; set; } = "0";

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
