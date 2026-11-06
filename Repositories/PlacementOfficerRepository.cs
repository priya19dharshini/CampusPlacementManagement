using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class PlacementOfficerRepository : IPlacementOfficerRepository
{
    private readonly ApplicationDbContext _context;

    public PlacementOfficerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PlacementOfficer?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return await _context.PlacementOfficers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                officer => officer.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        PlacementOfficer placementOfficer,
        CancellationToken cancellationToken)
    {
        await _context.PlacementOfficers.AddAsync(
            placementOfficer,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PlacementOfficer?> GetByIdWithUserAsync(
    int placementOfficerId,
    CancellationToken cancellationToken)
    {
        return await _context.PlacementOfficers
            .AsNoTracking()
            .Include(po => po.User)
            .FirstOrDefaultAsync(
                po => po.PlacementOfficerId == placementOfficerId,
                cancellationToken);
    }

    public async Task<PlacementOfficer?> GetByUserIdWithUserAsync(
    string userId,
    CancellationToken cancellationToken)
    {
        return await _context.PlacementOfficers
            .AsNoTracking()
            .Include(po => po.User)
            .FirstOrDefaultAsync(
                po => po.UserId == userId,
                cancellationToken);
    }
}
