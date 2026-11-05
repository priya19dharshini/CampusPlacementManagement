using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/company-hrs")]
[ApiController]
public class CompanyHRController : ControllerBase
{
    private readonly ICompanyHRService _companyHRService;

    public CompanyHRController(ICompanyHRService companyHRService)
    {
        _companyHRService = companyHRService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCompanyHR(
        [FromBody] CompanyHRRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _companyHRService.RegisterCompanyHRAsync(
                registrationDto,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Company not found",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Company HR registration failed",
                Detail = ex.Message
            });
        }
    }
}