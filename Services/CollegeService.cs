using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services;

public class CollegeService : ICollegeService
{
    private readonly ICollegeRepository _collegeRepository;

    public CollegeService(ICollegeRepository collegeRepository)
    {
        _collegeRepository = collegeRepository;
    }

    public async Task<CollegeResponseDto> RegisterCollegeAsync(
        CollegeRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        // Check whether the college code already exists.
        var existingCollege =
            await _collegeRepository.GetByCollegeCodeAsync(
                registrationDto.CollegeCode,
                cancellationToken);

        if (existingCollege is not null)
        {
            throw new InvalidOperationException(
                "A college with this college code already exists.");
        }

        // Create the college entity.
        var college = new College
        {
            CollegeName = registrationDto.CollegeName,
            CollegeCode = registrationDto.CollegeCode,
            UniversityName = registrationDto.UniversityName,
            OfficialEmail = registrationDto.OfficialEmail,
            PhoneNumber = registrationDto.PhoneNumber,
            Address = registrationDto.Address,
            Website = registrationDto.Website,
            Status = CollegeStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Save the college through the repository.
        await _collegeRepository.AddAsync(
            college,
            cancellationToken);

        // Return a response DTO.
        return new CollegeResponseDto
        {
            CollegeId = college.CollegeId,
            CollegeName = college.CollegeName,
            CollegeCode = college.CollegeCode,
            UniversityName = college.UniversityName,
            OfficialEmail = college.OfficialEmail,
            PhoneNumber = college.PhoneNumber,
            Address = college.Address,
            Website = college.Website,
            Status = college.Status,
            CreatedAt = college.CreatedAt
        };
    }


    public async Task<CollegeResponseDto> UpdateCollegeApprovalAsync(
        int collegeId,
        CollegeApprovalDto approvalDto,
        CancellationToken cancellationToken)
    {
        var college = await _collegeRepository.GetByIdForUpdateAsync(
            collegeId,
            cancellationToken);

        if (college is null)
        {
            throw new KeyNotFoundException("College not found.");
        }

        if (college.Status != CollegeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending colleges can be approved or rejected.");
        }

        college.Status = approvalDto.IsApproved
            ? CollegeStatus.Approved
            : CollegeStatus.Rejected;

        await _collegeRepository.UpdateAsync(
            college,
            cancellationToken);

        return new CollegeResponseDto
        {
            CollegeId = college.CollegeId,
            CollegeName = college.CollegeName,
            CollegeCode = college.CollegeCode,
            UniversityName = college.UniversityName,
            OfficialEmail = college.OfficialEmail,
            PhoneNumber = college.PhoneNumber,
            Address = college.Address,
            Website = college.Website,
            Status = college.Status,
            CreatedAt = college.CreatedAt
        };
    }

}