using CampusPlacementManagement.DTOs.Companies;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/companies")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCompany(
        [FromBody] CompanyRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _companyService.RegisterCompanyAsync(
                registrationDto,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Company registration conflict",
                Detail = ex.Message
            });
        }
    }
}