using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface IInterviewScheduleRepository
{
    Task<InterviewSchedule?> GetByIdAsync(
        int interviewScheduleId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<InterviewSchedule>> GetByApplicationIdAsync(
        int applicationId,
        CancellationToken cancellationToken);

    Task AddAsync(
        InterviewSchedule interviewSchedule,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        InterviewSchedule interviewSchedule,
        CancellationToken cancellationToken);
}