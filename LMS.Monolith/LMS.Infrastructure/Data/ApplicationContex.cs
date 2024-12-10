using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Course Configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.MaxEnrollment).IsRequired();
                entity.Property(e => e.CurrentEnrollment).IsRequired();

                // Course - Prerequisites relationship
                entity.HasMany(e => e.Prerequisites)
                    .WithOne(p => p.Course)
                    .HasForeignKey(p => p.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Student Configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Enrollment Configuration
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EnrollmentDate).IsRequired();
                entity.Property(e => e.Status).IsRequired();

                // Relationships
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint for student-course combination
                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
            });

            // CoursePrerequisite Configuration
            modelBuilder.Entity<CoursePrerequisite>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsMandatory).IsRequired();

                // Relationships
                entity.HasOne(p => p.PrerequisiteCourse)
                    .WithMany()
                    .HasForeignKey(p => p.PrerequisiteCourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint for course-prerequisite combination
                entity.HasIndex(p => new { p.CourseId, p.PrerequisiteCourseId }).IsUnique();
            });
        }
    }

}
