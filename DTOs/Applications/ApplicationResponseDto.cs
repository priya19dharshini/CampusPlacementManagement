using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.DTOs.Applications;

public class ApplicationResponseDto
{
    public int JobApplicationId { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int JobId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public ApplicationStatus Status { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
