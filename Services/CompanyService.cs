using CampusPlacementManagement.DTOs.Companies;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyResponseDto> RegisterCompanyAsync(
        CompanyRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        var existingCompany =
            await _companyRepository.GetByCompanyEmailAsync(
                registrationDto.CompanyEmail,
                cancellationToken);

        if (existingCompany is not null)
        {
            throw new InvalidOperationException(
                "A company with this email already exists.");
        }

        var company = new Company
        {
            CompanyName = registrationDto.CompanyName,
            CompanyEmail = registrationDto.CompanyEmail,
            Website = registrationDto.Website,
            Industry = registrationDto.Industry,
            Address = registrationDto.Address,
            Description = registrationDto.Description,
            Status = CompanyStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _companyRepository.AddAsync(
            company,
            cancellationToken);

        return new CompanyResponseDto
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            CompanyEmail = company.CompanyEmail,
            Website = company.Website,
            Industry = company.Industry,
            Address = company.Address,
            Description = company.Description,
            Status = company.Status,
            CreatedAt = company.CreatedAt
        };
    }
}