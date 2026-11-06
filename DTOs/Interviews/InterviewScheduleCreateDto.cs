using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Interviews;

public class InterviewScheduleCreateDto
{
    [Required]
    public int ApplicationId { get; set; }

    [Required]
    public int InterviewRoundId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    [MaxLength(500)]
    public string? MeetingLink { get; set; }

    [MaxLength(255)]
    public string? Venue { get; set; }
}