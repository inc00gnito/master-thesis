using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Student.API.Data;
using Student.API.DTOs;
using Student.API.Models;

namespace Student.API.Services
{
    public class StudentService : IStudentService
    {
        private readonly StudentContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            StudentContext context,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<StudentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            var students = await _context.Students.ToListAsync();
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public async Task<StudentDto> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> GetStudentByEmailAsync(string email)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto)
        {
            var student = _mapper.Map<Models.Student>(studentDto);

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new StudentCreatedEvent
            {
                Id = student.Id,
                Name = $"{student.FirstName} {student.LastName}",
                Email = student.Email
            });

            return _mapper.Map<StudentDto>(student);
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

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<StudentEnrollment>> GetStudentEnrollments(int studentId)
        {
            return await _context.StudentEnrollments
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetCompletedCourseIdsAsync(int studentId)
        {
            return await _context.StudentEnrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Completed)
                .Select(e => e.CourseId)
                .ToListAsync();
        }
    }
}