using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.DTOs.Applications;

public class ApplicationStatusUpdateDto
{
    [EnumDataType(typeof(ApplicationStatus))]
    public ApplicationStatus Status { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
