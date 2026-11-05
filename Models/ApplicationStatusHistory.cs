using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.Models;

public class ApplicationStatusHistory
{
    public int ApplicationStatusHistoryId { get; set; }

    public int JobApplicationId { get; set; }

    public ApplicationStatus? PreviousStatus { get; set; }

    public ApplicationStatus Status { get; set; }

    public string? Notes { get; set; }

    public string UpdatedByUserId { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public JobApplication JobApplication { get; set; } = null!;

    public ApplicationUser UpdatedByUser { get; set; } = null!;
}
