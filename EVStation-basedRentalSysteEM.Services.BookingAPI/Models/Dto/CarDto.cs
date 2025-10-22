using System.Text.Json.Serialization;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Models.Dto
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }

        // Trạng thái pháp lý hoặc trạng thái xe (enum)
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CarStatus Status { get; set; }

        // Trạng thái sẵn sàng đặt
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CarAvailability Availability { get; set; } = CarAvailability.Available;

        public int StationId { get; set; }
        public string Color { get; set; } = "Red";
        public int SeatCount { get; set; } = 4;
        public bool IsRegistered { get; set; } = true;

        // Optional
        public int? CurrentBookingId { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
    }

    // Enum trạng thái xe hiện tại
    public enum CarStatus
    {
        Active,
        Inactive,
        Maintenance
    }

    // Enum trạng thái sẵn sàng đặt
    public enum CarAvailability
    {
        Available,
        Booked,
        UnderMaintenance
    }
}
