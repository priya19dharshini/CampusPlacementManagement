using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class RecruitmentDriveRepository : IRecruitmentDriveRepository
{
    private readonly ApplicationDbContext _context;

    public RecruitmentDriveRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecruitmentDrive?> GetByIdAsync(
        int driveId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RecruitmentDrives
            .AsNoTracking()
            .FirstOrDefaultAsync(
                rd => rd.DriveId == driveId,
                cancellationToken);
    }

    public async Task AddAsync(
        RecruitmentDrive recruitmentDrive,
        CancellationToken cancellationToken = default)
    {
        await _context.RecruitmentDrives.AddAsync(
            recruitmentDrive,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        RecruitmentDrive recruitmentDrive,
        CancellationToken cancellationToken = default)
    {
        _context.RecruitmentDrives.Update(recruitmentDrive);

        await _context.SaveChangesAsync(cancellationToken);
    }
}