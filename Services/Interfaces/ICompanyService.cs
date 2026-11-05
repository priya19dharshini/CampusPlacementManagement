using CampusPlacementManagement.DTOs.Companies;

namespace CampusPlacementManagement.Services.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponseDto> RegisterCompanyAsync(
        CompanyRegistrationDto registrationDto,
        CancellationToken cancellationToken);
}