using CampusPlacementManagement.DTOs.Applications;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class ApplicationService : IApplicationService
{
    private static readonly ApplicationStatus[] CompanyHrStatuses =
    [
        ApplicationStatus.UnderReview,
        ApplicationStatus.Shortlisted,
        ApplicationStatus.Rejected,
        ApplicationStatus.InterviewScheduled,
        ApplicationStatus.Selected,
        ApplicationStatus.NotSelected
    ];

    private readonly IApplicationRepository _applicationRepository;
    private readonly ICompanyHRRepository _companyHRRepository;
    private readonly IPlacementOfficerRepository _placementOfficerRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationService(
        IApplicationRepository applicationRepository,
        ICompanyHRRepository companyHRRepository,
        IPlacementOfficerRepository placementOfficerRepository,
        UserManager<ApplicationUser> userManager)
    {
        _applicationRepository = applicationRepository;
        _companyHRRepository = companyHRRepository;
        _placementOfficerRepository = placementOfficerRepository;
        _userManager = userManager;
    }

    public async Task<ApplicationResponseDto> ApplyAsync(
        string userId,
        ApplicationCreateDto request,
        CancellationToken cancellationToken)
    {
        var student = await _applicationRepository.GetStudentForApplicationAsync(
            userId,
            cancellationToken);

        if (student is null)
        {
            throw new KeyNotFoundException("Student profile not found.");
        }

        if (student.User.AccountStatus != AccountStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only approved student accounts can apply for jobs.");
        }

        if (student.College.Status != CollegeStatus.Approved)
        {
            throw new InvalidOperationException(
                "Students from unapproved colleges cannot apply for jobs.");
        }

        var job = await _applicationRepository.GetJobForApplicationAsync(
            request.JobId,
            cancellationToken);

        if (job is null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        if (job.Status != JobStatus.Published)
        {
            throw new InvalidOperationException(
                "Applications are allowed only for published jobs.");
        }

        if (job.ApplicationDeadline <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "The application deadline has passed.");
        }

        if (job.Company.Status != CompanyStatus.Approved)
        {
            throw new InvalidOperationException(
                "Applications are allowed only for approved companies.");
        }

        if (!job.IsOpenToAllColleges &&
            !await _applicationRepository.IsCollegeTargetedAsync(
                job.JobId,
                student.CollegeId,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "The student's college is not eligible for this job.");
        }

        if (await _applicationRepository.HasEligibleDepartmentRulesAsync(
                job.JobId,
                cancellationToken) &&
            !await _applicationRepository.HasEligibleDepartmentAsync(
                job.JobId,
                student.Department,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "The student's department is not eligible for this job.");
        }

        if (await _applicationRepository.HasEligibleGraduationYearRulesAsync(
                job.JobId,
                cancellationToken) &&
            !await _applicationRepository.HasEligibleGraduationYearAsync(
                job.JobId,
                student.GraduationYear,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "The student's graduation year is not eligible for this job.");
        }

        if (job.MinimumCGPA.HasValue && student.CGPA < job.MinimumCGPA.Value)
        {
            throw new InvalidOperationException(
                "The student's CGPA does not meet the minimum requirement.");
        }

        if (job.MaximumBacklogs.HasValue &&
            student.ActiveBacklogs > job.MaximumBacklogs.Value)
        {
            throw new InvalidOperationException(
                "The student's active backlogs exceed the maximum allowed.");
        }

        var existingApplication = await _applicationRepository.GetByStudentAndJobAsync(
            student.StudentId,
            job.JobId,
            cancellationToken);

        if (existingApplication is not null)
        {
            throw new InvalidOperationException(
                "The student has already applied for this job.");
        }

        var application = new JobApplication
        {
            StudentId = student.StudentId,
            JobId = job.JobId,
            Status = ApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var history = new ApplicationStatusHistory
        {
            JobApplication = application,
            Status = ApplicationStatus.Applied,
            UpdatedByUserId = userId,
            UpdatedAt = application.AppliedAt
        };

        await _applicationRepository.AddAsync(
            application,
            history,
            cancellationToken);

        return Map(application, student.User.FullName, job);
    }

    public async Task<IReadOnlyList<ApplicationResponseDto>> GetMyApplicationsAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var student = await _applicationRepository.GetStudentForApplicationAsync(
            userId,
            cancellationToken);

        if (student is null)
        {
            throw new KeyNotFoundException("Student profile not found.");
        }

        var applications = await _applicationRepository.GetByStudentAsync(
            student.StudentId,
            cancellationToken);

        return applications.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<ApplicationResponseDto>> GetCompanyApplicationsAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var companyHr = await GetApprovedCompanyHrAsync(userId, cancellationToken);
        var applications = await _applicationRepository.GetByCompanyAsync(
            companyHr.CompanyId,
            cancellationToken);

        return applications.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<ApplicationResponseDto>> GetCollegeApplicationsAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var officer = await GetApprovedPlacementOfficerAsync(userId, cancellationToken);
        var applications = await _applicationRepository.GetByCollegeAsync(
            officer.CollegeId,
            cancellationToken);

        return applications.Select(Map).ToList();
    }

    public async Task<ApplicationResponseDto> GetApplicationAsync(
        string userId,
        string role,
        int jobApplicationId,
        CancellationToken cancellationToken)
    {
        var application = await GetAuthorizedApplicationAsync(
            userId,
            role,
            jobApplicationId,
            cancellationToken);

        return Map(application);
    }

    public async Task<ApplicationResponseDto> UpdateStatusAsync(
        string userId,
        string role,
        int jobApplicationId,
        ApplicationStatusUpdateDto request,
        CancellationToken cancellationToken)
    {
        var application = await GetAuthorizedApplicationAsync(
            userId,
            role,
            jobApplicationId,
            cancellationToken);

        if (role == "CompanyHR")
        {
            await GetApprovedCompanyHrAsync(userId, cancellationToken);

            if (!CompanyHrStatuses.Contains(request.Status))
            {
                throw new InvalidOperationException(
                    "CompanyHR cannot assign this application status.");
            }
        }
        else if (role != "SuperAdmin")
        {
            throw new UnauthorizedAccessException(
                "Only CompanyHR or SuperAdmin users can change application status.");
        }

        if (application.Status == request.Status)
        {
            throw new InvalidOperationException(
                "The application already has this status.");
        }

        var previousStatus = application.Status;
        application.Status = request.Status;
        application.UpdatedAt = DateTime.UtcNow;

        var history = new ApplicationStatusHistory
        {
            JobApplicationId = application.JobApplicationId,
            PreviousStatus = previousStatus,
            Status = request.Status,
            Notes = request.Notes,
            UpdatedByUserId = userId,
            UpdatedAt = application.UpdatedAt
        };

        await _applicationRepository.UpdateStatusAsync(
            application,
            history,
            cancellationToken);

        return Map(application);
    }

    public async Task<ApplicationResponseDto> WithdrawAsync(
        string userId,
        int jobApplicationId,
        CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdWithDetailsAsync(
            jobApplicationId,
            cancellationToken);

        if (application is null)
        {
            throw new KeyNotFoundException("Application not found.");
        }

        if (application.Student.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "Students can withdraw only their own applications.");
        }

        if (application.Status is not (ApplicationStatus.Applied or ApplicationStatus.UnderReview))
        {
            throw new InvalidOperationException(
                "Only Applied or UnderReview applications can be withdrawn.");
        }

        if (application.Job.ApplicationDeadline <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Applications cannot be withdrawn after the deadline.");
        }

        var previousStatus = application.Status;
        application.Status = ApplicationStatus.Withdrawn;
        application.UpdatedAt = DateTime.UtcNow;

        var history = new ApplicationStatusHistory
        {
            JobApplicationId = application.JobApplicationId,
            PreviousStatus = previousStatus,
            Status = ApplicationStatus.Withdrawn,
            UpdatedByUserId = userId,
            UpdatedAt = application.UpdatedAt
        };

        await _applicationRepository.UpdateStatusAsync(
            application,
            history,
            cancellationToken);

        return Map(application);
    }

    private async Task<JobApplication> GetAuthorizedApplicationAsync(
        string userId,
        string role,
        int jobApplicationId,
        CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdWithDetailsAsync(
            jobApplicationId,
            cancellationToken);

        if (application is null)
        {
            throw new KeyNotFoundException("Application not found.");
        }

        if (role == "Student")
        {
            if (application.Student.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "Students can access only their own applications.");
            }

            return application;
        }

        if (role == "CompanyHR")
        {
            var companyHr = await GetApprovedCompanyHrAsync(userId, cancellationToken);

            if (application.Job.CompanyId != companyHr.CompanyId)
            {
                throw new UnauthorizedAccessException(
                    "CompanyHR users can access only their company's applications.");
            }

            return application;
        }

        if (role == "PlacementOfficer")
        {
            var officer = await GetApprovedPlacementOfficerAsync(userId, cancellationToken);

            if (application.Student.CollegeId != officer.CollegeId)
            {
                throw new UnauthorizedAccessException(
                    "Placement officers can access only applications from their college.");
            }

            return application;
        }

        if (role == "SuperAdmin")
        {
            return application;
        }

        throw new UnauthorizedAccessException("The user role is not authorized.");
    }

    private async Task<CompanyHR> GetApprovedCompanyHrAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var companyHr = await _companyHRRepository.GetByUserIdWithCompanyAsync(
            userId,
            cancellationToken);

        if (companyHr is null)
        {
            throw new KeyNotFoundException("CompanyHR profile not found.");
        }

        if (companyHr.User.AccountStatus != AccountStatus.Approved ||
            companyHr.Company.Status != CompanyStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Only approved CompanyHR users from approved companies are authorized.");
        }

        return companyHr;
    }

    private async Task<PlacementOfficer> GetApprovedPlacementOfficerAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var officer = await _placementOfficerRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (officer is null)
        {
            throw new KeyNotFoundException("Placement officer profile not found.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || user.AccountStatus != AccountStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Only approved placement officers are authorized.");
        }

        return officer;
    }

    private static ApplicationResponseDto Map(JobApplication application)
    {
        return Map(
            application,
            application.Student.User.FullName,
            application.Job);
    }

    private static ApplicationResponseDto Map(
        JobApplication application,
        string studentName,
        JobOpening job)
    {
        return new ApplicationResponseDto
        {
            JobApplicationId = application.JobApplicationId,
            StudentId = application.StudentId,
            StudentName = studentName,
            JobId = application.JobId,
            JobTitle = job.JobTitle,
            CompanyId = job.CompanyId,
            CompanyName = job.Company.CompanyName,
            Status = application.Status,
            AppliedAt = application.AppliedAt,
            UpdatedAt = application.UpdatedAt
        };
    }
}
