using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface IJobRepository
{
    Task AddAsync(
        JobOpening job,
        IReadOnlyCollection<JobTargetCollege> targetColleges,
        IReadOnlyCollection<JobEligibleDepartment> eligibleDepartments,
        IReadOnlyCollection<JobEligibleGraduationYear> eligibleGraduationYears,
        CancellationToken cancellationToken);

    Task<JobOpening?> GetByIdAsync(
        int jobId,
        CancellationToken cancellationToken);

    Task<bool> HasTargetCollegeAsync(
        int jobId,
        int collegeId,
        CancellationToken cancellationToken);
}