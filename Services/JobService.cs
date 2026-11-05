using CampusPlacementManagement.DTOs.Jobs;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly ICompanyHRRepository _companyHRRepository;
    private readonly ICollegeRepository _collegeRepository;

    public JobService(
        IJobRepository jobRepository,
        ICompanyHRRepository companyHRRepository,
        ICollegeRepository collegeRepository)
    {
        _jobRepository = jobRepository;
        _companyHRRepository = companyHRRepository;
        _collegeRepository = collegeRepository;
    }

    public async Task<JobResponseDto> CreateJobAsync(
        string userId,
        JobRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        // 1. Verify CompanyHR account
        var companyHR =
            await _companyHRRepository.GetByUserIdWithCompanyAsync(
                userId,
                cancellationToken);

        if (companyHR is null)
        {
            throw new KeyNotFoundException(
                "CompanyHR profile not found.");
        }

        if (companyHR.User.AccountStatus != AccountStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only approved CompanyHR accounts can create jobs.");
        }

        // 2. Verify company approval
        if (companyHR.Company.Status != CompanyStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only approved companies can create jobs.");
        }

        // 3. Create JobOpening object FIRST
        var job = new JobOpening
        {
            CompanyId = companyHR.CompanyId,
            CreatedByHRId = companyHR.CompanyHRId,
            JobTitle = registrationDto.JobTitle,
            JobDescription = registrationDto.JobDescription,
            JobType = registrationDto.JobType,
            Location = registrationDto.Location,
            SalaryPackage = registrationDto.SalaryPackage,
            MinimumCGPA = registrationDto.MinimumCGPA,
            MaximumBacklogs = registrationDto.MaximumBacklogs,
            ApplicationDeadline = registrationDto.ApplicationDeadline,
            IsOpenToAllColleges =
                registrationDto.IsOpenToAllColleges,
            Status = JobStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        // 4. Validate target colleges
        if (registrationDto.IsOpenToAllColleges &&
            registrationDto.TargetCollegeIds.Count > 0)
        {
            throw new InvalidOperationException(
                "Target colleges cannot be specified when the job is open to all colleges.");
        }

        if (!registrationDto.IsOpenToAllColleges &&
            registrationDto.TargetCollegeIds.Count == 0)
        {
            throw new InvalidOperationException(
                "Select at least one target college for this job.");
        }

        var targetColleges = new List<JobTargetCollege>();

        foreach (var collegeId in registrationDto.TargetCollegeIds.Distinct())
        {
            var college = await _collegeRepository.GetByIdAsync(
                collegeId,
                cancellationToken);

            if (college is null)
            {
                throw new KeyNotFoundException(
                    $"College with ID {collegeId} was not found.");
            }

            if (college.Status != CollegeStatus.Approved)
            {
                throw new InvalidOperationException(
                    $"College with ID {collegeId} is not approved.");
            }

            targetColleges.Add(new JobTargetCollege
            {
                CollegeId = collegeId,
                Job = job
            });
        }

        // 5. Create eligible department records
        var eligibleDepartments =
            registrationDto.EligibleDepartments
                .Select(department => department.Trim())
                .Where(department =>
                    !string.IsNullOrWhiteSpace(department))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(department => new JobEligibleDepartment
                {
                    Department = department,
                    Job = job
                })
                .ToList();

        // 6. Create eligible graduation year records
        var eligibleGraduationYears =
            registrationDto.EligibleGraduationYears
                .Distinct()
                .Select(year => new JobEligibleGraduationYear
                {
                    GraduationYear = year,
                    Job = job
                })
                .ToList();

        // 7. Save job and eligibility records
        await _jobRepository.AddAsync(
            job,
            targetColleges,
            eligibleDepartments,
            eligibleGraduationYears,
            cancellationToken);

        // 8. Return response
        return new JobResponseDto
        {
            JobId = job.JobId,
            CompanyId = companyHR.CompanyId,
            CompanyName = companyHR.Company.CompanyName,
            CreatedByHRId = companyHR.CompanyHRId,
            JobTitle = job.JobTitle,
            JobDescription = job.JobDescription,
            JobType = job.JobType,
            Location = job.Location,
            SalaryPackage = job.SalaryPackage,
            MinimumCGPA = job.MinimumCGPA,
            MaximumBacklogs = job.MaximumBacklogs,
            ApplicationDeadline = job.ApplicationDeadline,
            IsOpenToAllColleges = job.IsOpenToAllColleges,
            Status = job.Status,
            CreatedAt = job.CreatedAt,

            TargetCollegeIds = targetColleges
                .Select(tc => tc.CollegeId)
                .ToList(),

            EligibleDepartments = eligibleDepartments
                .Select(ed => ed.Department)
                .ToList(),

            EligibleGraduationYears = eligibleGraduationYears
                .Select(eg => eg.GraduationYear)
                .ToList()
        };
    }
}
