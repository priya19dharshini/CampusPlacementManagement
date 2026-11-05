using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Jobs;

public class JobRegistrationDto
{
    [Required]
    [StringLength(200)]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    public string JobDescription { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string JobType { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999.99")]
    public decimal? SalaryPackage { get; set; }

    [Range(typeof(decimal), "0", "10")]
    public decimal? MinimumCGPA { get; set; }

    [Range(0, int.MaxValue)]
    public int? MaximumBacklogs { get; set; }

    [Required]
    public DateTime ApplicationDeadline { get; set; }

    public bool IsOpenToAllColleges { get; set; }

    public List<int> TargetCollegeIds { get; set; } = new();

    public List<string> EligibleDepartments { get; set; } = new();

    public List<int> EligibleGraduationYears { get; set; } = new();
}