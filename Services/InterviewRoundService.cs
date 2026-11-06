using CampusPlacementManagement.DTOs.Interviews;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services;

public class InterviewRoundService : IInterviewRoundService
{
    private readonly IInterviewRoundRepository _interviewRoundRepository;
    private readonly ICompanyHRRepository _companyHRRepository;
    private readonly IJobRepository _jobRepository;

    public InterviewRoundService(
        IInterviewRoundRepository interviewRoundRepository,
        ICompanyHRRepository companyHRRepository,
        IJobRepository jobRepository)
    {
        _interviewRoundRepository = interviewRoundRepository;
        _companyHRRepository = companyHRRepository;
        _jobRepository = jobRepository;
    }

    public async Task<InterviewRoundResponseDto> CreateRoundAsync(
        string userId,
        InterviewRoundCreateDto dto,
        CancellationToken cancellationToken)
    {
        // 1. Retrieve the CompanyHR with company and user details.
        var companyHR = await _companyHRRepository.GetByUserIdWithCompanyAsync(
            userId,
            cancellationToken);

        if (companyHR == null)
        {
            throw new KeyNotFoundException("CompanyHR profile not found.");
        }

        // 2. Check whether the CompanyHR account is approved.
        if (companyHR.User.AccountStatus != AccountStatus.Approved)
        {
            throw new InvalidOperationException(
                "Your account is not approved.");
        }

        // 3. Check whether the company is approved.
        if (companyHR.Company.Status != CompanyStatus.Approved)
        {
            throw new InvalidOperationException(
                "Your company is not approved.");
        }

        // 4. Retrieve the requested job.
        var job = await _jobRepository.GetByIdAsync(
            dto.JobId,
            cancellationToken);

        if (job == null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        // 5. Verify that the job belongs to the HR's company.
        if (job.CompanyId != companyHR.CompanyId)
        {
            throw new InvalidOperationException(
                "You are not authorized to manage interview rounds for this job.");
        }

        // 6. Create the interview round.
        var interviewRound = new InterviewRound
        {
            JobId = dto.JobId,
            RoundName = dto.RoundName,
            RoundNumber = dto.RoundNumber,
            Description = dto.Description
        };

        await _interviewRoundRepository.AddAsync(
            interviewRound,
            cancellationToken);

        // 7. Return the response DTO.
        return new InterviewRoundResponseDto
        {
            InterviewRoundId = interviewRound.InterviewRoundId,
            JobId = interviewRound.JobId,
            RoundName = interviewRound.RoundName,
            RoundNumber = interviewRound.RoundNumber,
            Description = interviewRound.Description
        };
    }
}