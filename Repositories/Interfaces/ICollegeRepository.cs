using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface ICollegeRepository
    {
        Task<College?> GetByCollegeCodeAsync(
            string collegeCode,
            CancellationToken cancellationToken);

        Task AddAsync(
            College college,
            CancellationToken cancellationToken);
    }
}
