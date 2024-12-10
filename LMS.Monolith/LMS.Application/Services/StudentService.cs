using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Core.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        public StudentService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto)
        {
            var student = _mapper.Map<Student>(studentDto);

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            var students = await _context.Students.ToListAsync();
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public Task<IEnumerable<int>> GetCompletedCourseIdsAsync(int studentId)
        {
            throw new NotImplementedException();
        }

        public async Task<StudentDto> GetStudentByEmailAsync(string email)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetStudentEnrollmentsAsync(int studentId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)  // Include course details
                .Include(e => e.Student) // Include student details
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                    CourseName = e.Course.Title,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status.ToString()
                })
                .ToListAsync();

            return enrollments;
        }

        public async Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto studentDto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return null;

            _mapper.Map(studentDto, student);
            student.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<StudentDto>(student);
        }
    }
}
