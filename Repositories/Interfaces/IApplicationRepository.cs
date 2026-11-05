using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface IApplicationRepository
{
    Task<Student?> GetStudentForApplicationAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<JobOpening?> GetJobForApplicationAsync(
        int jobId,
        CancellationToken cancellationToken);

    Task<bool> IsCollegeTargetedAsync(
        int jobId,
        int collegeId,
        CancellationToken cancellationToken);

    Task<bool> HasEligibleDepartmentAsync(
        int jobId,
        string department,
        CancellationToken cancellationToken);

    Task<bool> HasEligibleDepartmentRulesAsync(
        int jobId,
        CancellationToken cancellationToken);

    Task<bool> HasEligibleGraduationYearAsync(
        int jobId,
        int graduationYear,
        CancellationToken cancellationToken);

    Task<bool> HasEligibleGraduationYearRulesAsync(
        int jobId,
        CancellationToken cancellationToken);

    Task<JobApplication?> GetByStudentAndJobAsync(
        int studentId,
        int jobId,
        CancellationToken cancellationToken);

    Task<JobApplication?> GetByIdWithDetailsAsync(
        int jobApplicationId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<JobApplication>> GetByStudentAsync(
        int studentId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<JobApplication>> GetByCompanyAsync(
        int companyId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<JobApplication>> GetByCollegeAsync(
        int collegeId,
        CancellationToken cancellationToken);

    Task AddAsync(
        JobApplication application,
        ApplicationStatusHistory statusHistory,
        CancellationToken cancellationToken);

    Task UpdateStatusAsync(
        JobApplication application,
        ApplicationStatusHistory statusHistory,
        CancellationToken cancellationToken);
}
