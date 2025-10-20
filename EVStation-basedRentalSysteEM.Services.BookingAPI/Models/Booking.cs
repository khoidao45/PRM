using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVStation_basedRentalSysteEM.Services.BookingAPI.utils.enums;

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

       
        public string? HopDongId { get; set; } 

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
