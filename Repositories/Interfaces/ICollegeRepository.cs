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

        Task<College?> GetByIdAsync(
    int collegeId,
    CancellationToken cancellationToken);



        Task<College?> GetByIdForUpdateAsync(
            int collegeId,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            College college,
            CancellationToken cancellationToken);


    }
}
