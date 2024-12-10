using LMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Data
{
    public class DataSeeder
    {
        private readonly ApplicationContext _context;

        public DataSeeder(ApplicationContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (!_context.Courses.Any())
            {
                var courses = new List<Course>
            {
                new Course
                {
                    Title = "Introduction to Programming",
                    Description = "Basic programming concepts",
                    MaxEnrollment = 30,
                    CurrentEnrollment = 0
                },
                new Course
                {
                    Title = "Advanced Programming",
                    Description = "Advanced programming concepts",
                    MaxEnrollment = 25,
                    CurrentEnrollment = 0
                }
            };

                await _context.Courses.AddRangeAsync(courses);
                await _context.SaveChangesAsync();
            }

            if (!_context.Students.Any())
            {
                var students = new List<Student>
            {
                new Student
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com"
                },
                new Student
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com"
                }
            };

                await _context.Students.AddRangeAsync(students);
                await _context.SaveChangesAsync();
            }
        }
    }
}
