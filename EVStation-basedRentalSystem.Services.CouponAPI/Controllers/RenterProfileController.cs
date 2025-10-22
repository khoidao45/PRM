using System.Security.Claims;
using EVStation_basedRentalSystem.Services.AuthAPI.Models;
using EVStation_basedRentalSystem.Services.AuthAPI.Models.Dto.Request;
using EVStation_basedRentalSystem.Services.UserAPI.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RenterProfileController : ControllerBase
    {
        private readonly IRenterProfileService _service;

        public RenterProfileController(IRenterProfileService service)
        {
            _service = service;
        }

        // -------------------------------
        // Basic CRUD
        // -------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<RenterProfileDto>> GetById(string id)
        {
            var renter = await _service.GetByIdAsync(id);
            if (renter == null) return NotFound($"Renter {id} not found");

            return renter.ToDto(); // map entity -> DTO
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RenterProfileDto>>> GetAll()
        {
            var renters = await _service.GetAllAsync();
            return renters.Select(r => r.ToDto()).ToList();
        }
        [HttpPost("me")]
        [Authorize] // Bắt buộc user phải login
        public async Task<IActionResult> CreateOrUpdateMyProfile([FromBody] MyRenterProfileDto dto)
        {
            var userClaims = HttpContext.User;
            var profile = await _service.CreateOrUpdateMyProfileAsync(userClaims, dto);

            if (profile == null)
                return Unauthorized("User not authenticated or token invalid");

            return Ok(profile);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RenterProfile profile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(profile);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RenterProfile profile)
        {
            if (id != profile.Id)
                return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(profile);
            return updated == null ? NotFound($"Renter {id} not found") : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound($"Renter {id} not found");
        }

        // -------------------------------
        // Renter-Specific Actions
        // -------------------------------

        // Update Driver License Info (for renter self or staff verification)
        [HttpPost("{renterId}/driver-license")]
        public async Task<IActionResult> UpdateDriverLicense(string renterId, [FromBody] DriverLicenseDto dto)
        {
            var updated = await _service.UpdateDriverLicenseAsync(
                renterId,
                dto.LicenseNumber,
                dto.ImageUrl,
                dto.ExpiryDate,
                dto.LicenseClass
            );

            return updated == null
                ? NotFound($"Renter {renterId} not found")
                : Ok(updated);
        }

        // Update Identity Card Info
        [HttpPost("{renterId}/identity-card")]
        public async Task<IActionResult> UpdateIdentityCard(string renterId, [FromBody] IdentityCardDto dto)
        {
            var updated = await _service.UpdateIdentityCardAsync(
                renterId,
                dto.CardNumber,
                dto.ImageUrl,
                dto.IssuedDate,
                dto.IssuedPlace
            );

            return updated == null
                ? NotFound($"Renter {renterId} not found")
                : Ok(updated);
        }
    }


    // ---------------------------------------------
    // DTOs: mở rộng, dễ parse JSON, dễ test Swagger
    // ---------------------------------------------
    public record DriverLicenseDto(
        string LicenseNumber,
        string ImageUrl,
        DateTime? ExpiryDate = null,
        string? LicenseClass = null
    );

    public record IdentityCardDto(
        string CardNumber,
        string ImageUrl,
        DateTime? IssuedDate = null,
        string? IssuedPlace = null
    );
}
