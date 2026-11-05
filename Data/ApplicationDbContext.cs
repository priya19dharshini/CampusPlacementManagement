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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
        }


    }


}
