using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;

namespace CampusPlacementManagement.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;

    public JobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        JobOpening job,
        IReadOnlyCollection<JobTargetCollege> targetColleges,
        IReadOnlyCollection<JobEligibleDepartment> eligibleDepartments,
        IReadOnlyCollection<JobEligibleGraduationYear> eligibleGraduationYears,
        CancellationToken cancellationToken)
    {
        // Add the main job
        await _context.JobOpenings.AddAsync(
            job,
            cancellationToken);

        // Add target colleges
        await _context.JobTargetColleges.AddRangeAsync(
            targetColleges,
            cancellationToken);

        // Add eligible departments
        await _context.JobEligibleDepartments.AddRangeAsync(
            eligibleDepartments,
            cancellationToken);

        // Add eligible graduation years
        await _context.JobEligibleGraduationYears.AddRangeAsync(
            eligibleGraduationYears,
            cancellationToken);

        // Save everything together
        await _context.SaveChangesAsync(cancellationToken);
    }
}