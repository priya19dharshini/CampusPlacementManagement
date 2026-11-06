using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public JobApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobApplication?> GetByIdWithJobAsync(
        int jobApplicationId,
        CancellationToken cancellationToken)
    {
        return await _context.JobApplications
            .AsNoTracking()
            .Include(application => application.Job)
            .FirstOrDefaultAsync(
                application => application.JobApplicationId == jobApplicationId,
                cancellationToken);
    }
}