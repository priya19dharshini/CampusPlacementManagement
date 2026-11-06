using CampusPlacementManagement.DTOs.Interviews;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IInterviewScheduleService
{
    Task<InterviewScheduleResponseDto> ScheduleInterviewAsync(
        string userId,
        InterviewScheduleCreateDto dto,
        CancellationToken cancellationToken);
}