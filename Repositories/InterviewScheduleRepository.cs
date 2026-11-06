using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class InterviewScheduleRepository : IInterviewScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public InterviewScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InterviewSchedule?> GetByIdAsync(
        int interviewScheduleId,
        CancellationToken cancellationToken)
    {
        return await _context.InterviewSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(
                interview => interview.InterviewScheduleId == interviewScheduleId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<InterviewSchedule>> GetByApplicationIdAsync(
        int applicationId,
        CancellationToken cancellationToken)
    {
        return await _context.InterviewSchedules
            .AsNoTracking()
            .Where(interview => interview.ApplicationId == applicationId)
            .OrderBy(interview => interview.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        InterviewSchedule interviewSchedule,
        CancellationToken cancellationToken)
    {
        await _context.InterviewSchedules.AddAsync(
            interviewSchedule,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        InterviewSchedule interviewSchedule,
        CancellationToken cancellationToken)
    {
        _context.InterviewSchedules.Update(interviewSchedule);

        await _context.SaveChangesAsync(cancellationToken);
    }
}