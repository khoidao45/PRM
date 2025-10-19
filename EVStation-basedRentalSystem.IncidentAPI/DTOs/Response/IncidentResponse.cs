﻿namespace IncidentAPI.DTOs.Response
{
    public class IncidentResponse
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Description { get; set; }
        public List<string>? Images { get; set; }
        public DateTime ReportedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string Status { get; set; }
        public string? ResolutionNotes { get; set; }
        public decimal? CostIncurred { get; set; }
        public int? ResolvedBy { get; set; }
        public int ReportedBy { get; set; }
        public int? StationId { get; set; }

        // Additional info for better response
        public string? BookingInfo { get; set; }
        public string? ReporterName { get; set; }
        public string? ResolverName { get; set; }
    }
}
