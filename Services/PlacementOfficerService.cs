using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class PlacementOfficerService : IPlacementOfficerService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICollegeRepository _collegeRepository;
    private readonly IPlacementOfficerRepository _placementOfficerRepository;

    public PlacementOfficerService(
        UserManager<ApplicationUser> userManager,
        ICollegeRepository collegeRepository,
        IPlacementOfficerRepository placementOfficerRepository)
    {
        _userManager = userManager;
        _collegeRepository = collegeRepository;
        _placementOfficerRepository = placementOfficerRepository;
    }

    public async Task<PlacementOfficerResponseDto> RegisterPlacementOfficerAsync(
        PlacementOfficerRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        var college = await _collegeRepository.GetByIdAsync(
            registrationDto.CollegeId,
            cancellationToken);

        if (college is null)
        {
            throw new KeyNotFoundException(
    "College not found.");
        }

        if (college.Status != CollegeStatus.Approved)
        {
            throw new InvalidOperationException(
                "Placement officers can register only under approved colleges.");
        }

        var existingUser = await _userManager.FindByEmailAsync(
            registrationDto.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

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
                "; ",
                identityResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }

        var placementOfficer = new PlacementOfficer
        {
            UserId = user.Id,
            CollegeId = college.CollegeId,
            Designation = registrationDto.Designation
        };

        await _placementOfficerRepository.AddAsync(
            placementOfficer,
            cancellationToken);

        return new PlacementOfficerResponseDto
        {
            PlacementOfficerId = placementOfficer.PlacementOfficerId,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            CollegeId = placementOfficer.CollegeId,
            Designation = placementOfficer.Designation,
            AccountStatus = user.AccountStatus,
            ApprovedBy = null,
            ApprovedAt = null
        };
    }
}
