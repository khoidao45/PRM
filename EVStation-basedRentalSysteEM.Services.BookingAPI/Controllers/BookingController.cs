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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var responses = await _bookingService.GetAllBookingsAsync();
            return Ok(responses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _bookingService.GetBookingByIdAsync(id);
            if (response == null) return NotFound();
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingDTO dto)
        {
            var response = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var response = await _bookingService.UpdateBookingStatusAsync(id, status);
            if (response == null) return NotFound();
            return Ok(response);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUserId(string userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);

            if (bookings == null || !bookings.Any())
                return NotFound($"No bookings found for userId {userId}");

            return Ok(bookings);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
