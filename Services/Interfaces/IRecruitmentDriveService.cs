using CampusPlacementManagement.DTOs.RecruitmentDrives;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IRecruitmentDriveService
{
    Task<RecruitmentDriveResponseDto> CreateDriveAsync(
        string userId,
        RecruitmentDriveCreateDto dto,
        CancellationToken cancellationToken = default);
}