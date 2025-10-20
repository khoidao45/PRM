using EVStation_basedRentalSystem.Services.AuthAPI.Models;
using EVStation_basedRentalSystem.Services.UserAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffProfileController : ControllerBase
    {
        private readonly IStaffProfileService _staffService;
        private readonly IRenterProfileService _renterService; // Staff dùng để duyệt renter

        public StaffProfileController(
            IStaffProfileService staffService,
            IRenterProfileService renterService)
        {
            _staffService = staffService;
            _renterService = renterService;
        }

        // ===========================================================
        // SECTION 1: CRUD CHO STAFF
        // ===========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            return staff == null ? NotFound($"Staff {id} not found") : Ok(staff);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffProfile profile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _staffService.CreateAsync(profile);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] StaffProfile profile)
        {
            if (id != profile.Id) return BadRequest("ID mismatch");
            var updated = await _staffService.UpdateAsync(profile);
            return updated == null ? NotFound($"Staff {id} not found") : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _staffService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound($"Staff {id} not found");
        }

        // ===========================================================
        // SECTION 2: STAFF INTERNAL ACTIONS
        // ===========================================================
        [HttpPost("{staffId}/assign-shift")]
        public async Task<IActionResult> AssignShift(string staffId, [FromBody] string workShift)
        {
            var updated = await _staffService.AssignShiftAsync(staffId, workShift);
            return updated == null ? NotFound($"Staff {staffId} not found") : Ok(updated);
        }

        [HttpPost("{staffId}/assign-department")]
        public async Task<IActionResult> AssignDepartment(string staffId, [FromBody] string department)
        {
            var updated = await _staffService.AssignDepartmentAsync(staffId, department);
            return updated == null ? NotFound($"Staff {staffId} not found") : Ok(updated);
        }

        // ===========================================================
        // SECTION 3: STAFF DUYỆT / TỪ CHỐI HỒ SƠ RENTER
        // ===========================================================

        /// <summary>
        /// Staff duyệt hồ sơ renter (approve)
        /// </summary>
        [HttpPost("approve-renter/{renterId}")]
        public async Task<IActionResult> ApproveRenterProfile(string renterId)
        {
            var renter = await _renterService.ApproveRenterProfileAsync(renterId);
            if (renter == null)
                return NotFound($"Renter {renterId} not found");

            return Ok(new
            {
                message = "✅ Renter profile approved successfully",
                renter.Id,
                renter.LicenseStatus,
                renter.UpdatedAt
            });
        }

        /// <summary>
        /// Staff từ chối hồ sơ renter (reject)
        /// </summary>
        [HttpPost("reject-renter/{renterId}")]
        public async Task<IActionResult> RejectRenterProfile(string renterId, [FromBody] RejectRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest("Rejection reason is required.");

            var renter = await _renterService.RejectRenterProfileAsync(renterId, dto.Reason);
            if (renter == null)
                return NotFound($"Renter {renterId} not found");

            return Ok(new
            {
                message = "❌ Renter profile rejected",
                renter.Id,
                renter.LicenseStatus,
                renter.ReviewNote
            });
        }

        // ===========================================================
        // SECTION 4: DTO (Dùng cho request body)
        // ===========================================================
        public record RejectRequestDto(string Reason);
    }
}
