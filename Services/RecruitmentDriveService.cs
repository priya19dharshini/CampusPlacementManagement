using CampusPlacementManagement.DTOs.RecruitmentDrives;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services;

public class RecruitmentDriveService : IRecruitmentDriveService
{
    private readonly IRecruitmentDriveRepository _recruitmentDriveRepository;
    private readonly IPlacementOfficerRepository _placementOfficerRepository;
    private readonly ICollegeRepository _collegeRepository;
    private readonly IJobRepository _jobRepository;

    public RecruitmentDriveService(
        IRecruitmentDriveRepository recruitmentDriveRepository,
        IPlacementOfficerRepository placementOfficerRepository,
        ICollegeRepository collegeRepository,
        IJobRepository jobRepository)
    {
        _recruitmentDriveRepository = recruitmentDriveRepository;
        _placementOfficerRepository = placementOfficerRepository;
        _collegeRepository = collegeRepository;
        _jobRepository = jobRepository;
    }

    public async Task<RecruitmentDriveResponseDto> CreateDriveAsync(
        string userId,
        RecruitmentDriveCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        // 1. Get the Placement Officer with ApplicationUser
        var officer = await _placementOfficerRepository.GetByUserIdWithUserAsync(
            userId,
            cancellationToken);

        if (officer == null)
        {
            throw new InvalidOperationException(
                "Placement Officer profile not found.");
        }

        // 2. Check Placement Officer account status
        if (officer.User.AccountStatus != AccountStatus.Approved)
        {
            throw new InvalidOperationException(
                "Your account is not approved.");
        }

        // 3. Check College
        var college = await _collegeRepository.GetByIdAsync(
            officer.CollegeId,
            cancellationToken);

        if (college == null || college.Status != CollegeStatus.Approved)
        {
            throw new InvalidOperationException(
                "Your college is not approved.");
        }

        // 4. Ensure the requested college belongs to the officer
        if (dto.CollegeId != officer.CollegeId)
        {
            throw new InvalidOperationException(
                "You can create recruitment drives only for your own college.");
        }

        // 5. Validate Job
        var job = await _jobRepository.GetByIdAsync(
            dto.JobId,
            cancellationToken);

        if (job == null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        // 6. Check Job Status
        if (job.Status != JobStatus.Published)
        {
            throw new InvalidOperationException(
                "Recruitment drives can only be created for published jobs.");
        }

        // 7. Check Job College Eligibility
        if (!job.IsOpenToAllColleges)
        {
            var isCollegeEligible =
                await _jobRepository.HasTargetCollegeAsync(
                    job.JobId,
                    officer.CollegeId,
                    cancellationToken);

            if (!isCollegeEligible)
            {
                throw new InvalidOperationException(
                    "This job is not available for your college.");
            }
        }

        // 8. Validate Drive Date
        if (dto.DriveDate <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Recruitment drive date must be in the future.");
        }

        // 9. Create Recruitment Drive
        var recruitmentDrive = new RecruitmentDrive
        {
            JobId = job.JobId,
            CollegeId = officer.CollegeId,
            DriveDate = dto.DriveDate,
            Venue = dto.Venue,
            Description = dto.Description,
            Status = DriveStatus.Draft
        };

        await _recruitmentDriveRepository.AddAsync(
            recruitmentDrive,
            cancellationToken);

        // 10. Return Response DTO
        return new RecruitmentDriveResponseDto
        {
            DriveId = recruitmentDrive.DriveId,
            JobId = recruitmentDrive.JobId,
            CollegeId = recruitmentDrive.CollegeId,
            DriveDate = recruitmentDrive.DriveDate,
            Venue = recruitmentDrive.Venue,
            Description = recruitmentDrive.Description,
            Status = recruitmentDrive.Status
        };
    }
}