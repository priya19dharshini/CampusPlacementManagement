using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Applications;

public class ApplicationCreateDto
{
    [Range(1, int.MaxValue)]
    public int JobId { get; set; }
}
