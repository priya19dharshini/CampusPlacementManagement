using CampusPlacementManagement.DTOs.Students;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IStudentService
{
    Task<StudentResponseDto> EnrollStudentAsync(
        StudentRegistrationDto registrationDto,
        CancellationToken cancellationToken);
}