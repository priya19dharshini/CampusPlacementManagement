using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<Company?> GetByCompanyEmailAsync(
        string companyEmail,
        CancellationToken cancellationToken);

    Task AddAsync(
        Company company,
        CancellationToken cancellationToken);

    Task<Company?> GetByIdAsync(
    int companyId,
    CancellationToken cancellationToken);
}