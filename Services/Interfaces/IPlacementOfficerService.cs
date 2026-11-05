using CampusPlacementManagement.DTOs.PlacementOfficers;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IPlacementOfficerService
{
    Task<PlacementOfficerResponseDto> RegisterPlacementOfficerAsync(
        PlacementOfficerRegistrationDto registrationDto,
        CancellationToken cancellationToken);
}
