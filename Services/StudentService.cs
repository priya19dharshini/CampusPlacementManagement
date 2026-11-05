using CampusPlacementManagement.DTOs.Students;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class StudentService : IStudentService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICollegeRepository _collegeRepository;
    private readonly IPlacementOfficerRepository _placementOfficerRepository;
    private readonly IStudentRepository _studentRepository;

    public StudentService(
        UserManager<ApplicationUser> userManager,
        ICollegeRepository collegeRepository,
        IPlacementOfficerRepository placementOfficerRepository,
        IStudentRepository studentRepository)
    {
        _userManager = userManager;
        _collegeRepository = collegeRepository;
        _placementOfficerRepository = placementOfficerRepository;
        _studentRepository = studentRepository;
    }

    public async Task<StudentResponseDto> EnrollStudentAsync(
        StudentRegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        // 1. Verify college
        var college = await _collegeRepository.GetByIdAsync(
            registrationDto.CollegeId,
            cancellationToken);

        if (college is null)
        {
            throw new KeyNotFoundException("College not found.");
        }

        if (college.Status != CollegeStatus.Approved)
        {
            throw new InvalidOperationException(
                "Students can only be enrolled in approved colleges.");
        }

        // 2. Verify placement officer
        var placementOfficer =
            await _placementOfficerRepository.GetByIdWithUserAsync(
                registrationDto.PlacementOfficerId,
                cancellationToken);

        if (placementOfficer is null)
        {
            throw new KeyNotFoundException("Placement officer not found.");
        }

        if (placementOfficer.CollegeId != college.CollegeId)
        {
            throw new InvalidOperationException(
                "Placement officer does not belong to the selected college.");
        }

        if (placementOfficer.User.AccountStatus != AccountStatus.Approved)
        {
            throw new InvalidOperationException(
                "Placement officer account is not approved.");
        }

        // 3. Check duplicate register number
        var existingStudent =
            await _studentRepository.GetByCollegeAndRegisterNumberAsync(
                registrationDto.CollegeId,
                registrationDto.RegisterNumber,
                cancellationToken);

        if (existingStudent is not null)
        {
            throw new InvalidOperationException(
                "This register number is already registered in the college.");
        }

        // 4. Check existing email
        var existingUser = await _userManager.FindByEmailAsync(
            registrationDto.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        // 5. Create Identity user
        var user = new ApplicationUser
        {
            UserName = registrationDto.Email,
            Email = registrationDto.Email,
            PhoneNumber = registrationDto.PhoneNumber,
            FullName = registrationDto.FullName,
            AccountStatus = AccountStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var identityResult = await _userManager.CreateAsync(
            user,
            registrationDto.Password);

        if (!identityResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                identityResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }

        // 6. Create Student profile
        var student = new Student
        {
            UserId = user.Id,
            CollegeId = college.CollegeId,
            PlacementOfficerId = placementOfficer.PlacementOfficerId,
            RegisterNumber = registrationDto.RegisterNumber,
            Department = registrationDto.Department,
            Degree = registrationDto.Degree,
            GraduationYear = registrationDto.GraduationYear,
            CGPA = registrationDto.CGPA,
            ActiveBacklogs = registrationDto.ActiveBacklogs,
            ApprovedAt = null
        };

        await _studentRepository.AddAsync(student, cancellationToken);

        // 7. Return response
        return new StudentResponseDto
        {
            StudentId = student.StudentId,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            CollegeId = student.CollegeId,
            PlacementOfficerId = student.PlacementOfficerId,
            RegisterNumber = student.RegisterNumber,
            Department = student.Department,
            Degree = student.Degree,
            GraduationYear = student.GraduationYear,
            CGPA = student.CGPA,
            ActiveBacklogs = student.ActiveBacklogs,
            ResumeUrl = student.ResumeUrl,
            AccountStatus = user.AccountStatus,
            ApprovedAt = student.ApprovedAt
        };
    }
}