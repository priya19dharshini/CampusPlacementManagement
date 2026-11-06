using System.Security.Claims;
using CampusPlacementManagement.DTOs.Interviews;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/interviews")]
[ApiController]
[Authorize(Roles = "CompanyHR")]
public class InterviewController : ControllerBase
{
    private readonly IInterviewRoundService _interviewRoundService;
    private readonly IInterviewScheduleService _interviewScheduleService;

    public InterviewController(
        IInterviewRoundService interviewRoundService,
        IInterviewScheduleService interviewScheduleService)
    {
        _interviewRoundService = interviewRoundService;
        _interviewScheduleService = interviewScheduleService;
    }

    // POST: api/interviews/rounds
    [HttpPost("rounds")]
    public async Task<IActionResult> CreateRound(
        [FromBody] InterviewRoundCreateDto dto,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _interviewRoundService.CreateRoundAsync(
                userId,
                dto,
                cancellationToken);

            return CreatedAtAction(
                nameof(CreateRound),
                new { interviewRoundId = result.InterviewRoundId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid operation",
                detail: ex.Message);
        }
    }

    // POST: api/interviews/schedules
    [HttpPost("schedules")]
    public async Task<IActionResult> ScheduleInterview(
        [FromBody] InterviewScheduleCreateDto dto,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _interviewScheduleService.ScheduleInterviewAsync(
                userId,
                dto,
                cancellationToken);

            return CreatedAtAction(
                nameof(ScheduleInterview),
                new { interviewScheduleId = result.InterviewScheduleId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid operation",
                detail: ex.Message);
        }
    }
}