using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface IRecruitmentDriveRepository
{
    Task<RecruitmentDrive?> GetByIdAsync(
        int driveId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        RecruitmentDrive recruitmentDrive,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        RecruitmentDrive recruitmentDrive,
        CancellationToken cancellationToken = default);
}