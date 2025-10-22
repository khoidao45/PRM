namespace EVStation_basedRentalSystem.Services.CarAPI.Models.DTO
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int UserId { get; set; }
        public System.DateTime BookingDate { get; set; }
        public string Status { get; set; }
    }
}