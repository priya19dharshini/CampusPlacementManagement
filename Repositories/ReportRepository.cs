using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Reports;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Overall placement summary
    public async Task<PlacementSummaryResponseDto> GetPlacementSummaryAsync(
        CancellationToken cancellationToken)
    {
        return new PlacementSummaryResponseDto
        {
            TotalColleges = await _context.Colleges.CountAsync(cancellationToken),

            TotalCompanies = await _context.Companies.CountAsync(cancellationToken),

            TotalStudents = await _context.Students.CountAsync(cancellationToken),

            TotalJobs = await _context.JobOpenings.CountAsync(cancellationToken),

            PublishedJobs = await _context.JobOpenings
                .CountAsync(j => j.Status == JobStatus.Published, cancellationToken),

            TotalApplications = await _context.JobApplications.CountAsync(cancellationToken),

            SelectedApplications = await _context.JobApplications
                .CountAsync(a => a.Status == ApplicationStatus.Selected, cancellationToken),

            RejectedApplications = await _context.JobApplications
                .CountAsync(a => a.Status == ApplicationStatus.Rejected, cancellationToken)
        };
    }

    // 2. College-wise placement report
    public async Task<IReadOnlyList<CollegePlacementReportResponseDto>>
        GetCollegePlacementReportAsync(CancellationToken cancellationToken)
    {
        var colleges = await _context.Colleges
            .AsNoTracking()
            .Select(c => new
            {
                c.CollegeId,
                c.CollegeName,

                TotalStudents = _context.Students
                    .Count(s => s.CollegeId == c.CollegeId),

                TotalApplications = _context.JobApplications
                    .Count(a => a.Student.CollegeId == c.CollegeId),

                SelectedStudents = _context.JobApplications
                    .Where(a =>
                        a.Student.CollegeId == c.CollegeId &&
                        a.Status == ApplicationStatus.Selected)
                    .Select(a => a.StudentId)
                    .Distinct()
                    .Count(),

                NotSelectedStudents = _context.JobApplications
                    .Where(a =>
                        a.Student.CollegeId == c.CollegeId &&
                        a.Status == ApplicationStatus.NotSelected)
                    .Select(a => a.StudentId)
                    .Distinct()
                    .Count()
            })
            .ToListAsync(cancellationToken);

        return colleges.Select(c => new CollegePlacementReportResponseDto
        {
            CollegeId = c.CollegeId,
            CollegeName = c.CollegeName,
            TotalStudents = c.TotalStudents,
            TotalApplications = c.TotalApplications,
            SelectedStudents = c.SelectedStudents,
            NotSelectedStudents = c.NotSelectedStudents,
            PlacementPercentage = c.TotalStudents == 0
                ? 0
                : Math.Round(
                    (decimal)c.SelectedStudents / c.TotalStudents * 100, 2)
        }).ToList();
    }

    // 3. Company-wise placement report
    public async Task<IReadOnlyList<CompanyPlacementReportResponseDto>>
        GetCompanyPlacementReportAsync(CancellationToken cancellationToken)
    {
        return await _context.Companies
            .AsNoTracking()
            .Select(c => new CompanyPlacementReportResponseDto
            {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName,

                TotalJobsPosted = _context.JobOpenings
                    .Count(j => j.CompanyId == c.CompanyId),

                TotalApplicationsReceived = _context.JobApplications
                    .Count(a => a.Job.CompanyId == c.CompanyId),

                TotalSelectedStudents = _context.JobApplications
                    .Count(a =>
                        a.Job.CompanyId == c.CompanyId &&
                        a.Status == ApplicationStatus.Selected),

                TotalRejectedApplications = _context.JobApplications
                    .Count(a =>
                        a.Job.CompanyId == c.CompanyId &&
                        a.Status == ApplicationStatus.Rejected)
            })
            .ToListAsync(cancellationToken);
    }

    // 4. Job-wise application report
    public async Task<IReadOnlyList<JobApplicationReportResponseDto>>
        GetJobApplicationReportAsync(CancellationToken cancellationToken)
    {
        return await _context.JobOpenings
            .AsNoTracking()
            .Select(j => new JobApplicationReportResponseDto
            {
                JobId = j.JobId,
                JobTitle = j.JobTitle,

                TotalApplications = _context.JobApplications
                    .Count(a => a.JobId == j.JobId),

                AppliedApplications = _context.JobApplications
                    .Count(a =>
                        a.JobId == j.JobId &&
                        a.Status == ApplicationStatus.Applied),

                UnderReviewApplications = _context.JobApplications
                    .Count(a =>
                        a.JobId == j.JobId &&
                        a.Status == ApplicationStatus.UnderReview),

                ShortlistedApplications = _context.JobApplications
                    .Count(a =>
                        a.JobId == j.JobId &&
                        a.Status == ApplicationStatus.Shortlisted),

                SelectedApplications = _context.JobApplications
                    .Count(a =>
                        a.JobId == j.JobId &&
                        a.Status == ApplicationStatus.Selected),

                RejectedApplications = _context.JobApplications
                    .Count(a =>
                        a.JobId == j.JobId &&
                        a.Status == ApplicationStatus.Rejected),

                NotSelectedApplications = _context.JobApplications
                    .Count(a =>
                        a.JobId == j.JobId &&
                        a.Status == ApplicationStatus.NotSelected)
            })
            .ToListAsync(cancellationToken);
    }

    // 5. Student-wise placement report
    public async Task<IReadOnlyList<StudentPlacementReportResponseDto>>
        GetStudentPlacementReportAsync(
            int collegeId,
            CancellationToken cancellationToken)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.CollegeId == collegeId)
            .Select(s => new StudentPlacementReportResponseDto
            {
                StudentId = s.StudentId,
                RegisterNumber = s.RegisterNumber,
                StudentName = s.User.FullName,
                Department = s.Department,

                TotalApplications = _context.JobApplications
                    .Count(a => a.StudentId == s.StudentId),

                ShortlistedApplications = _context.JobApplications
                    .Count(a =>
                        a.StudentId == s.StudentId &&
                        a.Status == ApplicationStatus.Shortlisted),

                SelectedApplications = _context.JobApplications
                    .Count(a =>
                        a.StudentId == s.StudentId &&
                        a.Status == ApplicationStatus.Selected),

                RejectedApplications = _context.JobApplications
                    .Count(a =>
                        a.StudentId == s.StudentId &&
                        a.Status == ApplicationStatus.Rejected)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentPlacementReportResponseDto>>
    GetStudentPlacementReportAsync(
        int? collegeId,
        CancellationToken cancellationToken)
    {
        var query = _context.Students
            .AsNoTracking()
            .AsQueryable();

        if (collegeId.HasValue)
        {
            query = query.Where(s => s.CollegeId == collegeId.Value);
        }

        return await query
            .Select(s => new StudentPlacementReportResponseDto
            {
                StudentId = s.StudentId,
                RegisterNumber = s.RegisterNumber,
                StudentName = s.User.FullName,
                Department = s.Department,

                TotalApplications = _context.JobApplications
                    .Count(a => a.StudentId == s.StudentId),

                ShortlistedApplications = _context.JobApplications
                    .Count(a => a.StudentId == s.StudentId &&
                        a.Status == ApplicationStatus.Shortlisted),

                SelectedApplications = _context.JobApplications
                    .Count(a => a.StudentId == s.StudentId &&
                        a.Status == ApplicationStatus.Selected),

                RejectedApplications = _context.JobApplications
                    .Count(a => a.StudentId == s.StudentId &&
                        a.Status == ApplicationStatus.Rejected)
            })
            .ToListAsync(cancellationToken);
    }
}