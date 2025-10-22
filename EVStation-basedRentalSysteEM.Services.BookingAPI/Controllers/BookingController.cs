using EVStation_basedRentalSystem.Services.BookingAPI.Models;
using EVStation_basedRentalSystem.Services.BookingAPI.Models.DTO;
using EVStation_basedRentalSystem.Services.BookingAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace EVStation_basedRentalSystem.Services.BookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // -------------------- BASIC CRUD --------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingDTO dto)
        {
            var booking = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var booking = await _bookingService.UpdateBookingStatusAsync(id, status);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // -------------------- USER-SPECIFIC --------------------
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            if (bookings == null || !bookings.Any()) return NotFound();
            return Ok(bookings);
        }

        [HttpGet("user/{userId}/status")]
        public async Task<IActionResult> GetByUserAndStatus(string userId, [FromQuery] string status)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAndStatusAsync(userId, status);
            if (bookings == null || !bookings.Any()) return NotFound();
            return Ok(bookings);
        }

        // -------------------- STAFF-SPECIFIC --------------------
        [HttpGet("staff/bookings")]
        public async Task<IActionResult> GetBookingsForStaff(
            [FromQuery] string? userId,
            [FromQuery] string? status,
            [FromQuery] int? carId,
            [FromQuery] int? stationId,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end)
        {
            var bookings = await _bookingService.GetBookingsForStaffAsync(userId, status, carId, stationId, start, end);
            return Ok(bookings);
        }

        [HttpPut("staff/{bookingId}/status")]
        public async Task<IActionResult> UpdateBookingStatusByStaff(int bookingId, [FromQuery] string status)
        {
            var booking = await _bookingService.UpdateBookingStatusByStaffAsync(bookingId, status);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpGet("staff/checkin")]
        public async Task<IActionResult> GetBookingsRequiringCheckIn()
        {
            var bookings = await _bookingService.GetBookingsRequiringCheckInAsync();
            return Ok(bookings);
        }

        // -------------------- ADMIN-SPECIFIC --------------------
        [HttpGet("admin/filter")]
        public async Task<IActionResult> GetBookingsByFilter(
            [FromQuery] string? userId,
            [FromQuery] string? carId,
            [FromQuery] string? stationId,
            [FromQuery] string? status,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end)
        {
            var bookings = await _bookingService.GetBookingsByFilterAsync(userId, carId, stationId, status, start, end);
            return Ok(bookings);
        }

        [HttpGet("admin/history/{bookingId}")]
        public async Task<IActionResult> GetBookingHistory(int bookingId)
        {
            var history = await _bookingService.GetBookingHistoryAsync(bookingId);
            return Ok(history);
        }

        [HttpGet("admin/paginated")]
        public async Task<IActionResult> GetPaginatedBookings(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? userId = null)
        {
            var paginated = await _bookingService.GetPaginatedBookingsAsync(pageIndex, pageSize, status, userId);
            return Ok(paginated);
        }

        [HttpGet("admin/revenue")]
        public async Task<IActionResult> GetRevenue(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] string? stationId = null)
        {
            var revenue = await _bookingService.CalculateTotalRevenueAsync(start, end, stationId);
            return Ok(new { revenue });
        }

        [HttpGet("admin/active-count")]
        public async Task<IActionResult> GetActiveBookingCount()
        {
            var count = await _bookingService.CountActiveBookingsAsync();
            return Ok(new { count });
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetBookingDetail(int id)
        {
            var detail = await _bookingService.GetBookingDetailByIdAsync(id);
            if (detail == null) return NotFound();
            return Ok(detail);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetAllBookingDetails()
        {
            var details = await _bookingService.GetAllBookingDetailsAsync();
            return Ok(details);
        }
    }
}
