using Enrollment.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Enrollment.API.Data {

public class EnrollmentContext : DbContext
{
    public EnrollmentContext(DbContextOptions<EnrollmentContext> options) : base(options)
    {
    }

    public DbSet<Models.Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.Status).HasDefaultValue(EnrollmentStatus.Pending);
            
            // Ensure a student can't enroll in the same course twice
            entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
        });
    }
}
}