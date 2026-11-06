namespace CampusPlacementManagement.DTOs.Reports;

public class PlacementSummaryResponseDto
{
    public int TotalColleges { get; set; }

    public int TotalCompanies { get; set; }

    public int TotalStudents { get; set; }

    public int TotalJobs { get; set; }

    public int PublishedJobs { get; set; }

    public int TotalApplications { get; set; }

    public int SelectedApplications { get; set; }

    public int RejectedApplications { get; set; }
}