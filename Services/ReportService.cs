using CampusPlacementManagement.DTOs.Reports;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IPlacementOfficerRepository _placementOfficerRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReportService(
        IReportRepository reportRepository,
        IPlacementOfficerRepository placementOfficerRepository,
        UserManager<ApplicationUser> userManager)
    {
        _reportRepository = reportRepository;
        _placementOfficerRepository = placementOfficerRepository;
        _userManager = userManager;
    }

    public Task<PlacementSummaryResponseDto> GetPlacementSummaryAsync(
        CancellationToken cancellationToken)
    {
        return _reportRepository.GetPlacementSummaryAsync(cancellationToken);
    }

    public Task<IReadOnlyList<CollegePlacementReportResponseDto>>
        GetCollegePlacementReportAsync(
            CancellationToken cancellationToken)
    {
        return _reportRepository.GetCollegePlacementReportAsync(
            cancellationToken);
    }

    public Task<IReadOnlyList<CompanyPlacementReportResponseDto>>
        GetCompanyPlacementReportAsync(
            CancellationToken cancellationToken)
    {
        return _reportRepository.GetCompanyPlacementReportAsync(
            cancellationToken);
    }

    public Task<IReadOnlyList<JobApplicationReportResponseDto>>
        GetJobApplicationReportAsync(
            CancellationToken cancellationToken)
    {
        return _reportRepository.GetJobApplicationReportAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyList<StudentPlacementReportResponseDto>>
        GetStudentPlacementReportAsync(
            string userId,
            int? collegeId,
            CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("SuperAdmin"))
        {
            return await _reportRepository.GetStudentPlacementReportAsync(
                null,
                cancellationToken);
        }

        if (roles.Contains("PlacementOfficer"))
        {
            var placementOfficer =
                await _placementOfficerRepository.GetByUserIdAsync(
                    userId,
                    cancellationToken);

            if (placementOfficer is null)
            {
                throw new KeyNotFoundException(
                    "Placement officer profile not found.");
            }

            return await _reportRepository.GetStudentPlacementReportAsync(
                placementOfficer.CollegeId,
                cancellationToken);
        }

        throw new UnauthorizedAccessException(
            "You are not authorized to access student placement reports.");
    }
}