using CampusPlacementManagement.DTOs.Students;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/students")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollStudent(
        [FromBody] StudentRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _studentService.EnrollStudentAsync(
                registrationDto,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Enrollment resource not found",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Student enrollment failed",
                Detail = ex.Message
            });
        }
    }
}