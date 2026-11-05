using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
}