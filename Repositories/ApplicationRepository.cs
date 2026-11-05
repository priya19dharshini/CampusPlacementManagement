using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetStudentForApplicationAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return await _context.Students
            .Include(student => student.User)
            .Include(student => student.College)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                student => student.UserId == userId,
                cancellationToken);
    }

    public async Task<bool> HasEligibleDepartmentRulesAsync(
        int jobId,
        CancellationToken cancellationToken)
    {
        return await _context.JobEligibleDepartments
            .AsNoTracking()
            .AnyAsync(
                eligible => eligible.JobId == jobId,
                cancellationToken);
    }

    public async Task<JobOpening?> GetJobForApplicationAsync(
        int jobId,
        CancellationToken cancellationToken)
    {
        return await _context.JobOpenings
            .Include(job => job.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                job => job.JobId == jobId,
                cancellationToken);
    }

    public async Task<bool> HasEligibleGraduationYearRulesAsync(
        int jobId,
        CancellationToken cancellationToken)
    {
        return await _context.JobEligibleGraduationYears
            .AsNoTracking()
            .AnyAsync(
                eligible => eligible.JobId == jobId,
                cancellationToken);
    }

    public async Task<bool> IsCollegeTargetedAsync(
        int jobId,
        int collegeId,
        CancellationToken cancellationToken)
    {
        return await _context.JobTargetColleges
            .AsNoTracking()
            .AnyAsync(
                target => target.JobId == jobId && target.CollegeId == collegeId,
                cancellationToken);
    }

    public async Task<bool> HasEligibleDepartmentAsync(
        int jobId,
        string department,
        CancellationToken cancellationToken)
    {
        return await _context.JobEligibleDepartments
            .AsNoTracking()
            .AnyAsync(
                eligible => eligible.JobId == jobId && eligible.Department == department,
                cancellationToken);
    }

    public async Task<bool> HasEligibleGraduationYearAsync(
        int jobId,
        int graduationYear,
        CancellationToken cancellationToken)
    {
        return await _context.JobEligibleGraduationYears
            .AsNoTracking()
            .AnyAsync(
                eligible => eligible.JobId == jobId && eligible.GraduationYear == graduationYear,
                cancellationToken);
    }

    public async Task<JobApplication?> GetByStudentAndJobAsync(
        int studentId,
        int jobId,
        CancellationToken cancellationToken)
    {
        return await _context.JobApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                application => application.StudentId == studentId && application.JobId == jobId,
                cancellationToken);
    }

    public async Task<JobApplication?> GetByIdWithDetailsAsync(
        int jobApplicationId,
        CancellationToken cancellationToken)
    {
        return await _context.JobApplications
            .Include(application => application.Student)
                .ThenInclude(student => student.User)
            .Include(application => application.Student)
                .ThenInclude(student => student.College)
            .Include(application => application.Job)
                .ThenInclude(job => job.Company)
            .FirstOrDefaultAsync(
                application => application.JobApplicationId == jobApplicationId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<JobApplication>> GetByStudentAsync(
        int studentId,
        CancellationToken cancellationToken)
    {
        return await GetApplicationQuery()
            .AsNoTracking()
            .Where(application => application.StudentId == studentId)
            .OrderByDescending(application => application.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JobApplication>> GetByCompanyAsync(
        int companyId,
        CancellationToken cancellationToken)
    {
        return await GetApplicationQuery()
            .AsNoTracking()
            .Where(application => application.Job.CompanyId == companyId)
            .OrderByDescending(application => application.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JobApplication>> GetByCollegeAsync(
        int collegeId,
        CancellationToken cancellationToken)
    {
        return await GetApplicationQuery()
            .AsNoTracking()
            .Where(application => application.Student.CollegeId == collegeId)
            .OrderByDescending(application => application.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        JobApplication application,
        ApplicationStatusHistory statusHistory,
        CancellationToken cancellationToken)
    {
        await _context.JobApplications.AddAsync(application, cancellationToken);
        await _context.ApplicationStatusHistories.AddAsync(statusHistory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(
        JobApplication application,
        ApplicationStatusHistory statusHistory,
        CancellationToken cancellationToken)
    {
        _context.JobApplications.Update(application);
        await _context.ApplicationStatusHistories.AddAsync(statusHistory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<JobApplication> GetApplicationQuery()
    {
        return _context.JobApplications
            .Include(application => application.Student)
                .ThenInclude(student => student.User)
            .Include(application => application.Job)
                .ThenInclude(job => job.Company);
    }
}
