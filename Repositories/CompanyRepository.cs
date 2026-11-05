using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Company?> GetByCompanyEmailAsync(
        string companyEmail,
        CancellationToken cancellationToken)
    {
        return await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(
                company => company.CompanyEmail == companyEmail,
                cancellationToken);
    }

    public async Task AddAsync(
        Company company,
        CancellationToken cancellationToken)
    {
        await _context.Companies.AddAsync(
            company,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Company?> GetByIdAsync(
    int companyId,
    CancellationToken cancellationToken)
    {
        return await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(
                company => company.CompanyId == companyId,
                cancellationToken);
    }
}