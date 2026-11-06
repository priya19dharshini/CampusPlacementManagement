using CampusPlacementManagement.DTOs.Colleges;

namespace CampusPlacementManagement.Services.Interfaces;

public interface ICollegeService
{
    Task<CollegeResponseDto> RegisterCollegeAsync(
        CollegeRegistrationDto registrationDto,
        CancellationToken cancellationToken);


    Task<CollegeResponseDto> UpdateCollegeApprovalAsync(
        int collegeId,
        CollegeApprovalDto approvalDto,
        CancellationToken cancellationToken);

}