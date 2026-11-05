using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class CompanyHRService : ICompanyHRService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyHRRepository _companyHRRepository;

    public CompanyHRService(
        UserManager<ApplicationUser> userManager,
        ICompanyRepository companyRepository,
        ICompanyHRRepository companyHRRepository)
    {
        _userManager = userManager;
        _companyRepository = companyRepository;
        _companyHRRepository = companyHRRepository;
    }

    public async Task<CompanyHRResponseDto> RegisterCompanyHRAsync(
        CompanyHRRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        // 1. Verify company
        var company = await _companyRepository.GetByIdAsync(
            registrationDto.CompanyId,
            cancellationToken);

        if (company is null)
        {
            throw new KeyNotFoundException("Company not found.");
        }

        // 2. Check company approval
        if (company.Status != CompanyStatus.Approved)
        {
            throw new InvalidOperationException(
                "Company must be approved before HR registration.");
        }

        // 3. Check existing email
        var existingUser = await _userManager.FindByEmailAsync(
            registrationDto.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        // 4. Create Identity user
        var user = new ApplicationUser
        {
            UserName = registrationDto.Email,
            Email = registrationDto.Email,
            PhoneNumber = registrationDto.PhoneNumber,
            FullName = registrationDto.FullName,
            AccountStatus = AccountStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var identityResult = await _userManager.CreateAsync(
            user,
            registrationDto.Password);

        if (!identityResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                identityResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }

        // 5. Create CompanyHR profile
        var companyHR = new CompanyHR
        {
            UserId = user.Id,
            CompanyId = company.CompanyId,
            Designation = registrationDto.Designation
        };

        await _companyHRRepository.AddAsync(
            companyHR,
            cancellationToken);

        // 6. Return response
        return new CompanyHRResponseDto
        {
            CompanyHRId = companyHR.CompanyHRId,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            CompanyId = companyHR.CompanyId,
            Designation = companyHR.Designation,
            AccountStatus = user.AccountStatus,
            ApprovedBy = companyHR.ApprovedBy,
            ApprovedAt = companyHR.ApprovedAt
        };
    }
}