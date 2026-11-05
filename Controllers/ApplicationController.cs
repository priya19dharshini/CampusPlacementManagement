using System.Security.Claims;
using CampusPlacementManagement.DTOs.Applications;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/applications")]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost]
    public async Task<IActionResult> Apply(
        [FromBody] ApplicationCreateDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _applicationService.ApplyAsync(
                userId,
                request,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            return ToProblemDetails(ex, "Application submission failed");
        }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _applicationService.GetMyApplicationsAsync(
                userId,
                cancellationToken));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException)
        {
            return ToProblemDetails(ex, "Application access failed");
        }
    }

    [HttpGet("company")]
    public async Task<IActionResult> GetCompanyApplications(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _applicationService.GetCompanyApplicationsAsync(
                userId,
                cancellationToken));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException)
        {
            return ToProblemDetails(ex, "Company application access failed");
        }
    }

    [HttpGet("college")]
    public async Task<IActionResult> GetCollegeApplications(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _applicationService.GetCollegeApplicationsAsync(
                userId,
                cancellationToken));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException)
        {
            return ToProblemDetails(ex, "College application access failed");
        }
    }

    [HttpGet("{jobApplicationId:int}")]
    public async Task<IActionResult> GetApplication(
        int jobApplicationId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var role = GetRole();
        if (userId is null || role is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _applicationService.GetApplicationAsync(
                userId,
                role,
                jobApplicationId,
                cancellationToken));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException)
        {
            return ToProblemDetails(ex, "Application access failed");
        }
    }

    [HttpPatch("{jobApplicationId:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int jobApplicationId,
        [FromBody] ApplicationStatusUpdateDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var role = GetRole();
        if (userId is null || role is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _applicationService.UpdateStatusAsync(
                userId,
                role,
                jobApplicationId,
                request,
                cancellationToken));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException or UnauthorizedAccessException)
        {
            return ToProblemDetails(ex, "Application status update failed");
        }
    }

    [HttpPost("{jobApplicationId:int}/withdraw")]
    public async Task<IActionResult> Withdraw(
        int jobApplicationId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await _applicationService.WithdrawAsync(
                userId,
                jobApplicationId,
                cancellationToken));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException or UnauthorizedAccessException)
        {
            return ToProblemDetails(ex, "Application withdrawal failed");
        }
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private string? GetRole()
    {
        return User.FindFirstValue(ClaimTypes.Role)
            ?? User.FindFirstValue("role");
    }

    private ObjectResult ToProblemDetails(Exception exception, string title)
    {
        var status = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status409Conflict
        };

        return StatusCode(status, new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message
        });
    }
}
