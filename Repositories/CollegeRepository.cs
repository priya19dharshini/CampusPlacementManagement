using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CollegeRepository : ICollegeRepository
{
    private readonly ApplicationDbContext _context;

    public CollegeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<College?> GetByCollegeCodeAsync(
        string collegeCode,
        CancellationToken cancellationToken)
    {
        return await _context.Colleges
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.CollegeCode == collegeCode,
                cancellationToken);
    }

    public async Task AddAsync(
        College college,
        CancellationToken cancellationToken)
    {
        await _context.Colleges.AddAsync(
            college,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}