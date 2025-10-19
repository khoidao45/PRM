﻿using EVStation_basedRentalSystem.IncidentAPI.Data;
using IncidentAPI.DTOs.Request;
using IncidentAPI.DTOs.Response;
using IncidentAPI.Entities;
using IncidentAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace IncidentAPI.Services;

public class IncidentService : IIncidentService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly string _imageUploadPath;

    public IncidentService(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _context = context;
        _environment = environment;
        _configuration = configuration;
        _imageUploadPath = Path.Combine(_environment.WebRootPath, "uploads", "incidents");

        // Tạo thư mục nếu chưa tồn tại
        if (!Directory.Exists(_imageUploadPath))
        {
            Directory.CreateDirectory(_imageUploadPath);
        }
    }

    public async Task<IncidentResponse> CreateIncidentAsync(CreateIncidentFormRequest request)
    {
        // Validate booking exists
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId);

        if (booking == null)
        {
            throw new ArgumentException("Booking not found");
        }

        // Upload images nếu có
        List<string> imageUrls = new List<string>();

        // Tạo incident tạm để lấy ID
        var incident = new Incident
        {
            BookingId = request.BookingId,
            Description = request.Description,
            ReportedAt = DateTime.UtcNow,
            Status = "Pending",
            ReportedBy = request.ReportedBy,
            StationId = booking.PickupStationId
        };

        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync();

        // Upload images sau khi có ID
        if (request.Images != null && request.Images.Any())
        {
            imageUrls = await UploadImagesAsync(request.Images, incident.Id);
            incident.Images = System.Text.Json.JsonSerializer.Serialize(imageUrls);
            await _context.SaveChangesAsync();
        }

        return MapToResponse(incident);
    }

    public async Task<IncidentResponse?> UpdateIncidentAsync(int id, UpdateIncidentFormRequest request, int userId, string userRole)
    {
        var incident = await _context.Incidents.FindAsync(id);
        if (incident == null) return null;

        // Only Staff and Admin can update incidents
        if (userRole != "Staff" && userRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only staff and admin can update incidents");
        }

        // Xử lý upload ảnh mới
        List<string> currentImages = new List<string>();
        if (!string.IsNullOrEmpty(incident.Images))
        {
            currentImages = System.Text.Json.JsonSerializer.Deserialize<List<string>>(incident.Images) ?? new List<string>();
        }

        // Xóa ảnh cũ nếu có
        if (request.ImagesToRemove != null && request.ImagesToRemove.Any())
        {
            RemoveImages(request.ImagesToRemove);
            currentImages = currentImages.Where(img => !request.ImagesToRemove.Contains(img)).ToList();
        }

        // Thêm ảnh mới
        if (request.NewImages != null && request.NewImages.Any())
        {
            var uploadedImages = await UploadImagesAsync(request.NewImages, id);
            currentImages.AddRange(uploadedImages);
        }

        // Cập nhật danh sách ảnh
        incident.Images = System.Text.Json.JsonSerializer.Serialize(currentImages);

        // Update các field khác
        if (!string.IsNullOrEmpty(request.Status))
        {
            incident.Status = request.Status;

            // If status is resolved, set ResolvedAt
            if (request.Status == "Resolved" && !incident.ResolvedAt.HasValue)
            {
                incident.ResolvedAt = DateTime.UtcNow;
                incident.ResolvedBy = userId;
            }
        }

        if (request.ResolutionNotes != null)
        {
            incident.ResolutionNotes = request.ResolutionNotes;
        }

        if (request.CostIncurred.HasValue)
        {
            incident.CostIncurred = request.CostIncurred.Value;
        }

        if (request.ResolvedBy.HasValue && userRole == "Admin")
        {
            incident.ResolvedBy = request.ResolvedBy.Value;
        }

        await _context.SaveChangesAsync();

        return MapToResponse(incident);
    }

    // Các phương thức khác giữ nguyên...
    private async Task<List<string>> UploadImagesAsync(List<IFormFile> images, int incidentId)
    {
        var uploadedImageUrls = new List<string>();

        foreach (var image in images)
        {
            if (image.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException($"File type {fileExtension} is not allowed");
                }

                // Validate file size (max 5MB)
                if (image.Length > 5 * 1024 * 1024)
                {
                    throw new ArgumentException($"File {image.FileName} is too large. Maximum size is 5MB");
                }

                // Tạo tên file unique
                var fileName = $"{incidentId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(_imageUploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Lưu URL ảnh
                var imageUrl = $"/uploads/incidents/{fileName}";
                uploadedImageUrls.Add(imageUrl);
            }
        }

        return uploadedImageUrls;
    }

    private void RemoveImages(List<string> imageUrls)
    {
        foreach (var imageUrl in imageUrls)
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_imageUploadPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private static IncidentResponse MapToResponse(Incident incident)
    {
        return new IncidentResponse
        {
            Id = incident.Id,
            BookingId = incident.BookingId,
            Description = incident.Description,
            Images = !string.IsNullOrEmpty(incident.Images)
                ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(incident.Images)
                : new List<string>(),
            ReportedAt = incident.ReportedAt,
            ResolvedAt = incident.ResolvedAt,
            Status = incident.Status,
            ResolutionNotes = incident.ResolutionNotes,
            CostIncurred = incident.CostIncurred,
            ResolvedBy = incident.ResolvedBy,
            ReportedBy = incident.ReportedBy,
            StationId = incident.StationId
        };
    }

    // Các phương thức khác giữ nguyên...
    public async Task<IncidentListResponse> GetIncidentsAsync(int? stationId, string? status, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 20)
    {
        var query = _context.Incidents.AsQueryable();

        // Filter by station
        if (stationId.HasValue)
        {
            query = query.Where(i => i.StationId == stationId.Value);
        }

        // Filter by status
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(i => i.Status == status);
        }

        // Filter by date range
        if (dateFrom.HasValue)
        {
            query = query.Where(i => i.ReportedAt >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            query = query.Where(i => i.ReportedAt <= dateTo.Value);
        }

        // Pagination
        var totalCount = await query.CountAsync();
        var incidents = await query
            .OrderByDescending(i => i.ReportedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new IncidentListResponse
        {
            Incidents = incidents.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return response;
    }

    public async Task<IncidentResponse?> GetIncidentByIdAsync(int id, int userId, string userRole)
    {
        var incident = await _context.Incidents
            .FirstOrDefaultAsync(i => i.Id == id);

        if (incident == null) return null;

        // Authorization check
        if (userRole == "Renter" && incident.ReportedBy != userId)
        {
            throw new UnauthorizedAccessException("You can only view your own incidents");
        }

        return MapToResponse(incident);
    }

    public async Task<bool> ResolveIncidentAsync(int id, UpdateIncidentRequest request, int userId)
    {
        var incident = await _context.Incidents.FindAsync(id);
        if (incident == null) return false;

        incident.Status = "Resolved";
        incident.ResolvedAt = DateTime.UtcNow;
        incident.ResolvedBy = userId;

        if (!string.IsNullOrEmpty(request.ResolutionNotes))
        {
            incident.ResolutionNotes = request.ResolutionNotes;
        }

        if (request.CostIncurred.HasValue)
        {
            incident.CostIncurred = request.CostIncurred.Value;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteIncidentAsync(int id)
    {
        var incident = await _context.Incidents.FindAsync(id);
        if (incident == null) return false;

        _context.Incidents.Remove(incident);
        await _context.SaveChangesAsync();
        return true;
    }
}
