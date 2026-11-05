using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<Student?> GetByCollegeAndRegisterNumberAsync(
        int collegeId,
        string registerNumber,
        CancellationToken cancellationToken);

    Task AddAsync(
        Student student,
        CancellationToken cancellationToken);
}