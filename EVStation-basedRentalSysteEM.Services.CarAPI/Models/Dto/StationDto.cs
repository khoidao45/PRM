namespace EVStation_basedRentalSystem.Services.CarAPI.Models.DTO
{
    public class StationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public StationStatus Status { get; set; }
        public string ImageUrl { get; set; }
    }
}
public enum StationStatus { Active, Inactive, Maintenance }