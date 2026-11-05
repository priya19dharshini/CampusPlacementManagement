using CampusPlacementManagement.DTOs.Jobs;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IJobService
{
    Task<JobResponseDto> CreateJobAsync(
    string userId,
    JobRegistrationDto registrationDto,
    CancellationToken cancellationToken);
}