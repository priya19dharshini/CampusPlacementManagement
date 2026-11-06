using System.Security.Claims;
using CampusPlacementManagement.DTOs.RecruitmentDrives;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/recruitment-drives")]
[ApiController]
[Authorize(Roles = "PlacementOfficer")]
public class RecruitmentDriveController : ControllerBase
{
    private readonly IRecruitmentDriveService _recruitmentDriveService;

    public RecruitmentDriveController(
        IRecruitmentDriveService recruitmentDriveService)
    {
        _recruitmentDriveService = recruitmentDriveService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDrive(
        [FromBody] RecruitmentDriveCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _recruitmentDriveService.CreateDriveAsync(
                userId,
                dto,
                cancellationToken);

            return CreatedAtAction(
                nameof(CreateDrive),
                new { driveId = result.DriveId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Recruitment drive validation failed",
                Detail = ex.Message
            });
        }
    }
}