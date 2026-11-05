using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface ICompanyHRRepository
{
    Task<CompanyHR?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        CompanyHR companyHR,
        CancellationToken cancellationToken);

    Task<CompanyHR?> GetByUserIdWithCompanyAsync(
    string userId,
    CancellationToken cancellationToken);
}