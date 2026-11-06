using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.RecruitmentDrives;

public class RecruitmentDriveCreateDto
{
    [Required]
    public int JobId { get; set; }

    [Required]
    public int CollegeId { get; set; }

    [Required]
    public DateTime DriveDate { get; set; }

    [MaxLength(255)]
    public string? Venue { get; set; }

    public string? Description { get; set; }
}