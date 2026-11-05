using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.CompanyHRs;

public class CompanyHRRegistrationDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int CompanyId { get; set; }

    [StringLength(100)]
    public string? Designation { get; set; }
}