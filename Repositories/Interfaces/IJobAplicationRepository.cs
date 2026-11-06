using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface IJobApplicationRepository
{
    Task<JobApplication?> GetByIdWithJobAsync(
        int jobApplicationId,
        CancellationToken cancellationToken);
}