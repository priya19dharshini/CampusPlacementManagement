using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CampusPlacementManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollegeController : ControllerBase
{
    private readonly ICollegeService _collegeService;

    public CollegeController(ICollegeService collegeService)
    {
        _collegeService = collegeService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCollege(
        [FromBody] CollegeRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _collegeService.RegisterCollegeAsync(
                registrationDto,
                cancellationToken);

            return CreatedAtAction(
                nameof(RegisterCollege),
                new { id = result.CollegeId },
                result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "College registration conflict",
                Detail = ex.Message
            });
        }
    }


    [HttpPut("{collegeId:int}/approval")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateCollegeApproval(
        int collegeId,
        [FromBody] CollegeApprovalDto approvalDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result =
                await _collegeService.UpdateCollegeApprovalAsync(
                    collegeId,
                    approvalDto,
                    cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "College not found",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "College approval conflict",
                Detail = ex.Message
            });
        }
    }

}