using CampusPlacementManagement.DTOs.CompanyHRs;

namespace CampusPlacementManagement.Services.Interfaces;

public interface ICompanyHRService
{
    Task<CompanyHRResponseDto> RegisterCompanyHRAsync(
        CompanyHRRegistrationDto registrationDto,
        CancellationToken cancellationToken);
}