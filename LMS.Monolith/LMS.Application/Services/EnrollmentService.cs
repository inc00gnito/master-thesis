using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Core.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public EnrollmentService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CancelEnrollmentAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return false;

            enrollment.Status = EnrollmentStatus.Cancelled;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentDto enrollmentDto)
        {
            var prerequisites = await _context.CoursePrerequisites
                .Where(p => p.CourseId == enrollmentDto.CourseId)
                .ToListAsync();

            var studentEnrollments = await _context.Enrollments
                .Where(e => e.StudentId == enrollmentDto.StudentId && e.Status == EnrollmentStatus.Completed)
                .Select(e => e.CourseId)
                .ToListAsync();

            foreach (var prerequisite in prerequisites.Where(p => p.IsMandatory))
            {
                if (!studentEnrollments.Contains(prerequisite.PrerequisiteCourseId))
                {
                    throw new ApplicationException("Prerequisites not met");
                }
            }

            var enrollment = new Enrollment
            {
                StudentId = enrollmentDto.StudentId,
                CourseId = enrollmentDto.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return await GetEnrollmentAsync(enrollment.Id);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetAllEnrollmentsAsync()
        {
            var enrollments = await _context.Enrollments.ToListAsync();

            return _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
        }

        public async Task<EnrollmentDto> GetEnrollmentAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            return _mapper.Map<EnrollmentDto>(enrollment);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByCourseAsync(int courseId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByStudentAsync(int studentId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
        }

    }
}
