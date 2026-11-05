using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Students;

public class StudentRegistrationDto
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
    public int CollegeId { get; set; }

    [Required]
    public int PlacementOfficerId { get; set; }

    [Required]
    [StringLength(50)]
    public string RegisterNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Degree { get; set; } = string.Empty;

    [Required]
    [Range(2000, 2100)]
    public int GraduationYear { get; set; }

    [Required]
    [Range(typeof(decimal), "0", "10")]
    public decimal CGPA { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int ActiveBacklogs { get; set; }
}