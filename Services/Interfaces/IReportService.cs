using CampusPlacementManagement.DTOs.Reports;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IReportService
{
    Task<PlacementSummaryResponseDto> GetPlacementSummaryAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CollegePlacementReportResponseDto>> GetCollegePlacementReportAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CompanyPlacementReportResponseDto>> GetCompanyPlacementReportAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<JobApplicationReportResponseDto>> GetJobApplicationReportAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentPlacementReportResponseDto>> GetStudentPlacementReportAsync(
        string userId,
        int? collegeId,
        CancellationToken cancellationToken);
}