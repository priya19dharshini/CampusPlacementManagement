using CampusPlacementManagement.DTOs.Reports;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface IReportRepository
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
        int? collegeId,
        CancellationToken cancellationToken);
}