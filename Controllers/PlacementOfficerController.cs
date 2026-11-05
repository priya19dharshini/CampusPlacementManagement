
using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/placement-officers")]
[ApiController]
public class PlacementOfficerController : ControllerBase
{
    private readonly IPlacementOfficerService _placementOfficerService;

    public PlacementOfficerController(
        IPlacementOfficerService placementOfficerService)
    {
        _placementOfficerService = placementOfficerService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterPlacementOfficer(
        [FromBody] PlacementOfficerRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result =
                await _placementOfficerService.RegisterPlacementOfficerAsync(
                    registrationDto,
                    cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                result);
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
                Title = "Placement officer registration failed",
                Detail = ex.Message
            });
        }
    }
}
