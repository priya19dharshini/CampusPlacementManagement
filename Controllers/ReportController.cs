using System.Security.Claims;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/reports")]
[ApiController]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("placement-summary")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetPlacementSummary(
        CancellationToken cancellationToken)
    {
        var result = await _reportService.GetPlacementSummaryAsync(
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("colleges")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetCollegePlacementReport(
        CancellationToken cancellationToken)
    {
        var result = await _reportService.GetCollegePlacementReportAsync(
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("companies")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetCompanyPlacementReport(
        CancellationToken cancellationToken)
    {
        var result = await _reportService.GetCompanyPlacementReportAsync(
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("jobs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetJobApplicationReport(
        CancellationToken cancellationToken)
    {
        var result = await _reportService.GetJobApplicationReportAsync(
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("students")]
    [Authorize(Roles = "SuperAdmin,PlacementOfficer")]
    public async Task<IActionResult> GetStudentPlacementReport(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _reportService.GetStudentPlacementReportAsync(
                userId,
                null,
                cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "User or placement officer profile not found",
                Detail = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }
}