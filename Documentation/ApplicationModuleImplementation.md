# Application Module Implementation

## Status

Implemented the Application module through the controller, service, repository, EF Core configuration, and dependency-injection registration.

A workspace-wide build was not executed because the build request was rejected. Migration generation, database update, and API calls remain pending manual verification.

## Created files

- `Enums/ApplicationStatus.cs`
- `Models/JobApplication.cs`
- `Models/ApplicationStatusHistory.cs`
- `DTOs/Applications/ApplicationCreateDto.cs`
- `DTOs/Applications/ApplicationStatusUpdateDto.cs`
- `DTOs/Applications/ApplicationResponseDto.cs`
- `Repositories/Interfaces/IApplicationRepository.cs`
- `Repositories/ApplicationRepository.cs`
- `Services/Interfaces/IApplicationService.cs`
- `Services/ApplicationService.cs`
- `Controllers/ApplicationController.cs`

## Modified files

- `Data/ApplicationDbContext.cs`
  - Added application DbSets.
  - Added string enum conversion for application statuses.
  - Added the unique `(StudentId, JobId)` index.
  - Added restrictive relationships for applications and status history.
- `Program.cs`
  - Registered the application repository and service with scoped lifetime.

## Implemented business rules

- Applications require an approved student account and approved college.
- Applications require a published job, an approved company, and an unexpired deadline.
- College, department, graduation year, CGPA, and active-backlog eligibility are enforced.
- Duplicate applications are prevented by service validation and the database unique index.
- Initial application status history is created with every application.
- Every status update and withdrawal creates status history.
- CompanyHR access is restricted to the HR user's approved company.
- PlacementOfficer access is restricted to students from the officer's college.
- Students can access and withdraw only their own applications.
- CompanyHR can assign only `UnderReview`, `Shortlisted`, `Rejected`, `InterviewScheduled`, `Selected`, or `NotSelected`.
- Withdrawal is allowed only for `Applied` or `UnderReview` applications before the deadline.
- Reapplication is blocked by the unique constraint; an administrator must reset the application status.

## Migration commands

Run from the solution/project directory after confirming the project builds:

```powershell
dotnet ef migrations add ApplicationModule --project CampusPlacementManagement\CampusPlacementManagement.csproj --startup-project CampusPlacementManagement\CampusPlacementManagement.csproj
dotnet ef database update --project CampusPlacementManagement\CampusPlacementManagement.csproj --startup-project CampusPlacementManagement\CampusPlacementManagement.csproj
```

## Manual verification

Build:

```powershell
dotnet build CampusPlacementManagement\CampusPlacementManagement.csproj
```

Run the API:

```powershell
dotnet run --project CampusPlacementManagement\CampusPlacementManagement.csproj
```

Verify at minimum:

1. Approved student submits an eligible published job and receives `201 Created`.
2. Pending student, expired job, unpublished job, ineligible college, department, graduation year, CGPA, and backlog cases are rejected.
3. A duplicate submission is rejected.
4. Student can list and withdraw only their own application before the deadline while status is `Applied` or `UnderReview`.
5. CompanyHR can list and update applications only for the HR user's company.
6. PlacementOfficer can list and view applications only for the officer's college.
7. Each status transition creates an `ApplicationStatusHistory` row.

## Pending limitations

- Full workspace build was not run because the build request was rejected.
- Migration generation and database update were not run.
- API calls were not run.
- JWT and role registration remain intentionally outside this phase, as requested.
