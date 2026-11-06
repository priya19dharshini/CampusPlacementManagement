using CampusPlacementManagement.DTOs.Interviews;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services;

public class InterviewScheduleService : IInterviewScheduleService
{
    private readonly IInterviewScheduleRepository _interviewScheduleRepository;
    private readonly ICompanyHRRepository _companyHRRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IInterviewRoundRepository _interviewRoundRepository;

    public InterviewScheduleService(
        IInterviewScheduleRepository interviewScheduleRepository,
        ICompanyHRRepository companyHRRepository,
        IJobApplicationRepository jobApplicationRepository,
        IInterviewRoundRepository interviewRoundRepository)
    {
        _interviewScheduleRepository = interviewScheduleRepository;
        _companyHRRepository = companyHRRepository;
        _jobApplicationRepository = jobApplicationRepository;
        _interviewRoundRepository = interviewRoundRepository;
    }

    public async Task<InterviewScheduleResponseDto> ScheduleInterviewAsync(
        string userId,
        InterviewScheduleCreateDto dto,
        CancellationToken cancellationToken)
    {
        // 1. Retrieve the CompanyHR profile.
        var companyHR = await _companyHRRepository.GetByUserIdWithCompanyAsync(
            userId,
            cancellationToken);

        if (companyHR == null)
        {
            throw new KeyNotFoundException("CompanyHR profile not found.");
        }

        // 2. Verify the HR account.
        if (companyHR.User.AccountStatus != AccountStatus.Approved)
        {
            throw new InvalidOperationException(
                "Your account is not approved.");
        }

        // 3. Verify the company.
        if (companyHR.Company.Status != CompanyStatus.Approved)
        {
            throw new InvalidOperationException(
                "Your company is not approved.");
        }

        // 4. Retrieve the job application with its job.
        var application = await _jobApplicationRepository.GetByIdWithJobAsync(
            dto.ApplicationId,
            cancellationToken);

        if (application == null)
        {
            throw new KeyNotFoundException("Job application not found.");
        }

        // 5. Verify that the job belongs to the HR's company.
        if (application.Job.CompanyId != companyHR.CompanyId)
        {
            throw new InvalidOperationException(
                "You are not authorized to schedule interviews for this application.");
        }

        // 6. Retrieve the selected interview round.
        var interviewRound = await _interviewRoundRepository.GetByIdAsync(
            dto.InterviewRoundId,
            cancellationToken);

        if (interviewRound == null)
        {
            throw new KeyNotFoundException("Interview round not found.");
        }

        // 7. Verify that the round belongs to the application's job.
        if (interviewRound.JobId != application.JobId)
        {
            throw new InvalidOperationException(
                "The selected interview round does not belong to this job.");
        }

        // 8. Create the interview schedule.
        var interviewSchedule = new InterviewSchedule
        {
            ApplicationId = application.JobApplicationId,
            InterviewRoundId = interviewRound.InterviewRoundId,
            ScheduledAt = dto.ScheduledAt,
            MeetingLink = dto.MeetingLink,
            Venue = dto.Venue,
            Status = InterviewStatus.Scheduled
        };

        await _interviewScheduleRepository.AddAsync(
            interviewSchedule,
            cancellationToken);

        // 9. Return the response DTO.
        return new InterviewScheduleResponseDto
        {
            InterviewScheduleId = interviewSchedule.InterviewScheduleId,
            ApplicationId = interviewSchedule.ApplicationId,
            InterviewRoundId = interviewSchedule.InterviewRoundId,
            ScheduledAt = interviewSchedule.ScheduledAt,
            MeetingLink = interviewSchedule.MeetingLink,
            Venue = interviewSchedule.Venue,
            Status = interviewSchedule.Status,
            Feedback = interviewSchedule.Feedback
        };
    }
}