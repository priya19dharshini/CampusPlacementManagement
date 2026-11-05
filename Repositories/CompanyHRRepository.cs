using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CompanyHRRepository : ICompanyHRRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyHRRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyHR?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return await _context.CompanyHRs
            .AsNoTracking()
            .FirstOrDefaultAsync(
                hr => hr.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        CompanyHR companyHR,
        CancellationToken cancellationToken)
    {
        await _context.CompanyHRs.AddAsync(
            companyHR,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}