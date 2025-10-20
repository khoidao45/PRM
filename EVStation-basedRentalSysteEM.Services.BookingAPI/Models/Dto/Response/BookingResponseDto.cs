using System.ComponentModel.DataAnnotations.Schema;
using EVStation_basedRentalSysteEM.Services.BookingAPI.utils.enums;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Models.DTO
{
    public class BookingResponseDTO
    {
        public int Id { get; set; }            // Booking ID as int
        public string UserId { get; set; } 
        public string? HopDongId { get; set; }
        public string? UserName { get; set; }
        public int CarId { get; set; }
        public string? CarName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDTime { get; set; }
        public decimal TotalPrice { get; set; }


        [Column(TypeName = "nvarchar(30)")] 
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
    }
}
