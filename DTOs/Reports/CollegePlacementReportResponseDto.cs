namespace CampusPlacementManagement.DTOs.Reports;

public class CollegePlacementReportResponseDto
{
    public int CollegeId { get; set; }

    public string CollegeName { get; set; } = string.Empty;

    public int TotalStudents { get; set; }

    public int TotalApplications { get; set; }

    public int SelectedStudents { get; set; }

    public int NotSelectedStudents { get; set; }

    public decimal PlacementPercentage { get; set; }
}