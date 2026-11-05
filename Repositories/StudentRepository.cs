using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(
                student => student.UserId == userId,
                cancellationToken);
    }

    public async Task<Student?> GetByCollegeAndRegisterNumberAsync(
        int collegeId,
        string registerNumber,
        CancellationToken cancellationToken)
    {
        return await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(
                student =>
                    student.CollegeId == collegeId &&
                    student.RegisterNumber == registerNumber,
                cancellationToken);
    }

    public async Task AddAsync(
        Student student,
        CancellationToken cancellationToken)
    {
        await _context.Students.AddAsync(
            student,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}