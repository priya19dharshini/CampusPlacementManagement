using CampusPlacementManagement.DTOs.Applications;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IApplicationService
{
    Task<ApplicationResponseDto> ApplyAsync(
        string userId,
        ApplicationCreateDto request,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ApplicationResponseDto>> GetMyApplicationsAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ApplicationResponseDto>> GetCompanyApplicationsAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ApplicationResponseDto>> GetCollegeApplicationsAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<ApplicationResponseDto> GetApplicationAsync(
        string userId,
        string role,
        int jobApplicationId,
        CancellationToken cancellationToken);

    Task<ApplicationResponseDto> UpdateStatusAsync(
        string userId,
        string role,
        int jobApplicationId,
        ApplicationStatusUpdateDto request,
        CancellationToken cancellationToken);

    Task<ApplicationResponseDto> WithdrawAsync(
        string userId,
        int jobApplicationId,
        CancellationToken cancellationToken);
}
