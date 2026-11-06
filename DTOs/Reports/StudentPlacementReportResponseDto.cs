namespace CampusPlacementManagement.DTOs.Reports;

public class StudentPlacementReportResponseDto
{
    public int StudentId { get; set; }

    public string RegisterNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public int TotalApplications { get; set; }

    public int ShortlistedApplications { get; set; }

    public int SelectedApplications { get; set; }

    public int RejectedApplications { get; set; }
}