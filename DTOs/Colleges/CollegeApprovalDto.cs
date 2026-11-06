using System.ComponentModel.DataAnnotations;
namespace CampusPlacementManagement.DTOs.Colleges
{
    public class CollegeApprovalDto
    {
        [Required]
        public bool IsApproved { get; set; }
    }
}
