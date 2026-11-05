using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CampusPlacementManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<College> Colleges { get; set; }

        public DbSet<PlacementOfficer> PlacementOfficers { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<CompanyHR> CompanyHRs { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<JobOpening> JobOpenings { get; set; }

        public DbSet<JobTargetCollege> JobTargetColleges { get; set; }
        public DbSet<JobEligibleDepartment> JobEligibleDepartments { get; set; }
        public DbSet<JobEligibleGraduationYear> JobEligibleGraduationYears { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<JobOpening>(entity =>
            {
                entity.HasKey(j => j.JobId);

                entity.Property(j => j.JobTitle)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(j => j.JobDescription)
                    .IsRequired();

                entity.Property(j => j.JobType)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(j => j.Location)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(j => j.SalaryPackage)
                    .HasPrecision(12, 2);

                entity.Property(j => j.MinimumCGPA)
                    .HasPrecision(4, 2);

                entity.Property(j => j.ApplicationDeadline)
                    .IsRequired();

                entity.Property(j => j.IsOpenToAllColleges)
                    .IsRequired();

                entity.Property(j => j.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(j => j.CreatedAt)
                    .IsRequired();

                entity.HasOne(j => j.Company)
                    .WithMany()
                    .HasForeignKey(j => j.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(j => j.CreatedByHR)
                    .WithMany()
                    .HasForeignKey(j => j.CreatedByHRId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Job Target College
            builder.Entity<JobTargetCollege>(entity =>
            {
                entity.HasKey(jtc => jtc.JobTargetCollegeId);

                entity.HasIndex(jtc => new { jtc.JobId, jtc.CollegeId })
                    .IsUnique();

                entity.HasOne(jtc => jtc.Job)
                    .WithMany()
                    .HasForeignKey(jtc => jtc.JobId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(jtc => jtc.College)
                    .WithMany()
                    .HasForeignKey(jtc => jtc.CollegeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Job Eligible Department
            builder.Entity<JobEligibleDepartment>(entity =>
            {
                entity.HasKey(jed => jed.JobEligibleDepartmentId);

                entity.Property(jed => jed.Department)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(jed => new { jed.JobId, jed.Department })
                    .IsUnique();

                entity.HasOne(jed => jed.Job)
                    .WithMany()
                    .HasForeignKey(jed => jed.JobId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Job Eligible Graduation Year
            builder.Entity<JobEligibleGraduationYear>(entity =>
            {
                entity.HasKey(jgy => jgy.JobEligibleGraduationYearId);

                entity.HasIndex(jgy => new { jgy.JobId, jgy.GraduationYear })
                    .IsUnique();

                entity.HasOne(jgy => jgy.Job)
                    .WithMany()
                    .HasForeignKey(jgy => jgy.JobId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<College>(entity =>
            {
                entity.HasKey(c => c.CollegeId);

                entity.Property(c => c.CollegeName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(c => c.CollegeCode)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(c => c.CollegeCode)
                    .IsUnique();

                entity.Property(c => c.UniversityName)
                    .HasMaxLength(200);

                entity.Property(c => c.OfficialEmail)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(c => c.PhoneNumber)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(c => c.Address)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(c => c.Website)
                    .HasMaxLength(255);

                entity.Property(c => c.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(c => c.CreatedAt)
                    .IsRequired();
            });

            builder.Entity<PlacementOfficer>(entity =>
            {
                entity.HasKey(po => po.PlacementOfficerId);

                entity.Property(po => po.UserId)
                    .IsRequired();

                entity.HasIndex(po => po.UserId)
                    .IsUnique();

                entity.Property(po => po.Designation)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(po => po.ApprovedBy)
                    .IsRequired(false);

                // PlacementOfficer -> ApplicationUser (one-to-one)
                entity.HasOne(po => po.User)
                    .WithOne()
                    .HasForeignKey<PlacementOfficer>(po => po.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // PlacementOfficer -> College (many-to-one)
                entity.HasOne(po => po.College)
                    .WithMany()
                    .HasForeignKey(po => po.CollegeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // PlacementOfficer -> Approving ApplicationUser
                entity.HasOne(po => po.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(po => po.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.CompanyId);

                entity.Property(c => c.CompanyName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(c => c.CompanyEmail)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(c => c.Website)
                    .HasMaxLength(255);

                entity.Property(c => c.Industry)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(c => c.Address)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(c => c.Description);

                entity.Property(c => c.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(c => c.CreatedAt)
                    .IsRequired();
            });

            builder.Entity<CompanyHR>(entity =>
            {
                entity.HasKey(hr => hr.CompanyHRId);

                entity.Property(hr => hr.UserId)
                    .IsRequired();

                entity.HasIndex(hr => hr.UserId)
                    .IsUnique();

                entity.Property(hr => hr.Designation)
                    .HasMaxLength(100);

                entity.Property(hr => hr.ApprovedBy)
                    .IsRequired(false);

                entity.HasOne(hr => hr.User)
                    .WithOne()
                    .HasForeignKey<CompanyHR>(hr => hr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(hr => hr.Company)
                    .WithMany()
                    .HasForeignKey(hr => hr.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(hr => hr.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(hr => hr.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);

                entity.Property(s => s.UserId)
                    .IsRequired();

                entity.HasIndex(s => s.UserId)
                    .IsUnique();

                entity.Property(s => s.RegisterNumber)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(s => new { s.CollegeId, s.RegisterNumber })
                    .IsUnique();

                entity.Property(s => s.Department)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(s => s.Degree)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(s => s.GraduationYear)
                    .IsRequired();

                entity.Property(s => s.CGPA)
                    .HasPrecision(4, 2)
                    .IsRequired();

                entity.Property(s => s.ActiveBacklogs)
                    .IsRequired();

                entity.Property(s => s.ResumeUrl)
                    .HasMaxLength(500);

                entity.Property(s => s.ApprovedAt)
                    .IsRequired(false);

                entity.HasOne(s => s.User)
                    .WithOne()
                    .HasForeignKey<Student>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.College)
                    .WithMany()
                    .HasForeignKey(s => s.CollegeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.PlacementOfficer)
                    .WithMany()
                    .HasForeignKey(s => s.PlacementOfficerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }


    }


}
