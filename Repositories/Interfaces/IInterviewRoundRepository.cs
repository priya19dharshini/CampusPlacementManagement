using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface IInterviewRoundRepository
{
    Task<InterviewRound?> GetByIdAsync(
        int interviewRoundId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<InterviewRound>> GetByJobIdAsync(
        int jobId,
        CancellationToken cancellationToken);

    Task AddAsync(
        InterviewRound interviewRound,
        CancellationToken cancellationToken);
}