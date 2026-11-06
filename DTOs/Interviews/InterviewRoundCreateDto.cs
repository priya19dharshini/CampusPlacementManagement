using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Interviews;

public class InterviewRoundCreateDto
{
    [Required]
    public int JobId { get; set; }

    [Required]
    [MaxLength(100)]
    public string RoundName { get; set; } = string.Empty;

    [Required]
    [Range(1, 20)]
    public int RoundNumber { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}