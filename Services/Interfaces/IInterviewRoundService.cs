using CampusPlacementManagement.DTOs.Interviews;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IInterviewRoundService
{
    Task<InterviewRoundResponseDto> CreateRoundAsync(
        string userId,
        InterviewRoundCreateDto dto,
        CancellationToken cancellationToken);
}