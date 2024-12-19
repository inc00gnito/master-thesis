using Course.API.Models;

namespace Course.API.Data
{
    public class DataSeeder
    {
        private readonly CourseContext _context;

        public DataSeeder(CourseContext context)
        {
            _context = context;
        }

        public async Task SeedData()
        {
            if (!_context.Courses.Any())
            {
                // Create courses
                var courses = new List<Models.Course>
            {
                // Math sequence
                new Models.Course
                {
                    Title = "Basic Mathematics",
                    Description = "Fundamental mathematical concepts and operations",
                    MaxEnrollment = 30,
                    CurrentEnrollment = 0
                },
                new Models.Course
                {
                    Title = "Pre-Algebra",
                    Description = "Preparation for algebraic concepts",
                    MaxEnrollment = 25,
                    CurrentEnrollment = 0
                },
                new Models.Course
                {
                    Title = "Algebra I",
                    Description = "Introduction to algebraic concepts and equations",
                    MaxEnrollment = 25,
                    CurrentEnrollment = 0
                },
                new Models.Course
                {
                    Title = "Algebra II",
                    Description = "Advanced algebraic concepts and functions",
                    MaxEnrollment = 20,
                    CurrentEnrollment = 0
                },
                new Models.Course
                {
                    Title = "Algebra III",
                    Description = "Complex algebraic concepts and analysis",
                    MaxEnrollment = 15,
                    CurrentEnrollment = 0
                },
                
                // Programming sequence
                new Models.Course
                {
                    Title = "Programming Basics",
                    Description = "Introduction to programming concepts",
                    MaxEnrollment = 30,
                    CurrentEnrollment = 0
                },
                new Models.Course
                {
                    Title = "Intermediate Programming",
                    Description = "Object-oriented programming concepts",
                    MaxEnrollment = 25,
                    CurrentEnrollment = 0
                },
                new Models.Course
                {
                    Title = "Advanced Programming",
                    Description = "Advanced programming techniques and patterns",
                    MaxEnrollment = 20,
                    CurrentEnrollment = 0
                },

                // Database sequence
                new Models.Course
                {
                    Title = "Database Fundamentals",
                    Description = "Basic database concepts and SQL",
                    MaxEnrollment = 30,
                    CurrentEnrollment = 0
                },
                new Models.Course
                {
                    Title = "Advanced Database Design",
                    Description = "Complex database design and optimization",
                    MaxEnrollment = 20,
                    CurrentEnrollment = 0
                }
            };

                await _context.Courses.AddRangeAsync(courses);
                await _context.SaveChangesAsync();

                // Add prerequisites
                var prerequisites = new List<CoursePrerequisite>
            {
                // Math prerequisites
                new CoursePrerequisite
                {
                    CourseId = courses[2].Id, // Algebra I
                    PrerequisiteCourseId = courses[1].Id, // Pre-Algebra
                    PrerequisiteCourseName = courses[1].Title,
                    IsMandatory = true
                },
                new CoursePrerequisite
                {
                    CourseId = courses[3].Id, // Algebra II
                    PrerequisiteCourseId = courses[2].Id, // Algebra I
                    PrerequisiteCourseName = courses[2].Title,
                    IsMandatory = true
                },
                new CoursePrerequisite
                {
                    CourseId = courses[4].Id, // Algebra III
                    PrerequisiteCourseId = courses[3].Id, // Algebra II
                    PrerequisiteCourseName = courses[3].Title,
                    IsMandatory = true
                },

                // Programming prerequisites
                new CoursePrerequisite
                {
                    CourseId = courses[6].Id, // Intermediate Programming
                    PrerequisiteCourseId = courses[5].Id, // Programming Basics
                    PrerequisiteCourseName = courses[5].Title,
                    IsMandatory = true
                },
                new CoursePrerequisite
                {
                    CourseId = courses[7].Id, // Advanced Programming
                    PrerequisiteCourseId = courses[6].Id, // Intermediate Programming
                    PrerequisiteCourseName = courses[6].Title,
                    IsMandatory = true
                },

                // Database prerequisites
                new CoursePrerequisite
                {
                    CourseId = courses[9].Id, // Advanced Database Design
                    PrerequisiteCourseId = courses[8].Id, // Database Fundamentals
                    PrerequisiteCourseName = courses[8].Title,
                    IsMandatory = true
                }
            };

                await _context.CoursePrerequisites.AddRangeAsync(prerequisites);
                await _context.SaveChangesAsync();
            }
        }
    }

}
