namespace CampusPlacementManagement.DTOs.Reports;

public class JobApplicationReportResponseDto
{
    public int JobId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public int TotalApplications { get; set; }

    public int AppliedApplications { get; set; }

    public int UnderReviewApplications { get; set; }

    public int ShortlistedApplications { get; set; }

    public int SelectedApplications { get; set; }

    public int RejectedApplications { get; set; }

    public int NotSelectedApplications { get; set; }
}