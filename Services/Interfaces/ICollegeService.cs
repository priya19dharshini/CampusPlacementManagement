using CampusPlacementManagement.DTOs.Colleges;

namespace CampusPlacementManagement.Services.Interfaces;

public interface ICollegeService
{
    Task<CollegeResponseDto> RegisterCollegeAsync(
        CollegeRegistrationDto registrationDto,
        CancellationToken cancellationToken);


}