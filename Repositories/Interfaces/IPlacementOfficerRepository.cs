using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface IPlacementOfficerRepository
    {
        Task<PlacementOfficer?> GetByUserIdAsync(
            string userId,
            CancellationToken cancellationToken);

        Task AddAsync(
            PlacementOfficer placementOfficer,
            CancellationToken cancellationToken);

        Task<PlacementOfficer?> GetByIdWithUserAsync(
    int placementOfficerId,
    CancellationToken cancellationToken);
    }
}
