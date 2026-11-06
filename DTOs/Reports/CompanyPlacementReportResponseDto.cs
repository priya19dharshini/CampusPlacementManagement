namespace CampusPlacementManagement.DTOs.Reports;

public class CompanyPlacementReportResponseDto
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public int TotalJobsPosted { get; set; }

    public int TotalApplicationsReceived { get; set; }

    public int TotalSelectedStudents { get; set; }

    public int TotalRejectedApplications { get; set; }
}