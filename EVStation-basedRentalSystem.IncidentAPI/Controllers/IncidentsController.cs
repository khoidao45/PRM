﻿using IncidentAPI.DTOs.Request;
using IncidentAPI.DTOs.Response;
using IncidentAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVStation_basedRentalSystem.IncidentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly IWebHostEnvironment _environment;

        public IncidentsController(IIncidentService incidentService, IWebHostEnvironment environment)
        {
            _incidentService = incidentService;
            _environment = environment;
        }

        [HttpPost]
        public async Task<ActionResult<IncidentResponse>> CreateIncident([FromForm] CreateIncidentFormRequest request)
        {
            try
            {
                var incident = await _incidentService.CreateIncidentAsync(request);
                return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the incident", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IncidentListResponse>> GetIncidents(
            [FromQuery] int? stationId,
            [FromQuery] string? status,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // In real implementation, get user role from JWT token
                var userRole = User.FindFirst("role")?.Value ?? "Staff"; // Temporary for demo

                var incidents = await _incidentService.GetIncidentsAsync(stationId, status, dateFrom, dateTo, page, pageSize);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving incidents", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IncidentResponse>> GetIncident(int id)
        {
            try
            {
                // In real implementation, get user info from JWT token
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var userRole = User.FindFirst("role")?.Value ?? "Renter";

                var incident = await _incidentService.GetIncidentByIdAsync(id, userId, userRole);

                if (incident == null)
                {
                    return NotFound(new { message = "Incident not found" });
                }

                return Ok(incident);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the incident", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IncidentResponse>> UpdateIncident(
            int id,
            [FromForm] UpdateIncidentFormRequest request)
        {
            try
            {
                // In real implementation, get user info from JWT token
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var userRole = User.FindFirst("role")?.Value ?? "Staff";

                var incident = await _incidentService.UpdateIncidentAsync(id, request, userId, userRole);

                if (incident == null)
                {
                    return NotFound(new { message = "Incident not found" });
                }

                return Ok(incident);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the incident", error = ex.Message });
            }
        }

        [HttpPatch("{id}/resolve")]
        public async Task<ActionResult> ResolveIncident(int id, [FromBody] UpdateIncidentRequest request)
        {
            try
            {
                // In real implementation, get user info from JWT token
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");

                var success = await _incidentService.ResolveIncidentAsync(id, request, userId);

                if (!success)
                {
                    return NotFound(new { message = "Incident not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while resolving the incident", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIncident(int id)
        {
            try
            {
                var success = await _incidentService.DeleteIncidentAsync(id);

                if (!success)
                {
                    return NotFound(new { message = "Incident not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the incident", error = ex.Message });
            }
        }

        [HttpGet("download-image/{imageName}")]
        public async Task<IActionResult> DownloadImage(string imageName)
        {
            try
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "uploads", "incidents", imageName);

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound(new { message = "Image not found" });
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(imagePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var contentType = GetContentType(imagePath);
                return File(memory, contentType, Path.GetFileName(imagePath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while downloading the image", error = ex.Message });
            }
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
        {
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" }
        };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}
