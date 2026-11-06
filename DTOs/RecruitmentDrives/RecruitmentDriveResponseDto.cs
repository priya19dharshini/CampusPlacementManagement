using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.DTOs.RecruitmentDrives;

public class RecruitmentDriveResponseDto
{
    public int DriveId { get; set; }

    public int JobId { get; set; }

    public int CollegeId { get; set; }

    public DateTime DriveDate { get; set; }

    public string? Venue { get; set; }

    public string? Description { get; set; }

    public DriveStatus Status { get; set; }
}