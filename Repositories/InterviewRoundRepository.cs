using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class InterviewRoundRepository : IInterviewRoundRepository
{
    private readonly ApplicationDbContext _context;

    public InterviewRoundRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InterviewRound?> GetByIdAsync(
        int interviewRoundId,
        CancellationToken cancellationToken)
    {
        return await _context.InterviewRounds
            .AsNoTracking()
            .FirstOrDefaultAsync(
                ir => ir.InterviewRoundId == interviewRoundId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<InterviewRound>> GetByJobIdAsync(
        int jobId,
        CancellationToken cancellationToken)
    {
        return await _context.InterviewRounds
            .AsNoTracking()
            .Where(ir => ir.JobId == jobId)
            .OrderBy(ir => ir.RoundNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        InterviewRound interviewRound,
        CancellationToken cancellationToken)
    {
        await _context.InterviewRounds.AddAsync(
            interviewRound,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}