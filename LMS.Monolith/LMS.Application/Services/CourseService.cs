using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Core.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public CourseService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PrerequisiteDto> AddPrerequisiteAsync(int courseId, AddPrerequisiteDto dto)
        {
            // Validate that the course exists
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new NotFoundException($"Course {courseId} not found");

            // Validate that the prerequisite course exists
            var prerequisiteCourse = await _context.Courses.FindAsync(dto.PrerequisiteCourseId);
            if (prerequisiteCourse == null)
                throw new NotFoundException($"Prerequisite course {dto.PrerequisiteCourseId} not found");


            var prerequisite = new CoursePrerequisite
            {
                CourseId = courseId,
                PrerequisiteCourseId = dto.PrerequisiteCourseId,
                IsMandatory = dto.IsMandatory,
            };

            _context.CoursePrerequisites.Add(prerequisite);
            await _context.SaveChangesAsync();

            return new PrerequisiteDto
            {
                CourseId = prerequisite.CourseId,
                PrerequisiteCourseId = prerequisite.PrerequisiteCourseId,
                IsMandatory = prerequisite.IsMandatory,
                PrerequisiteCourseName = prerequisiteCourse.Title
            };
        }
        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto)
        {
            var course = _mapper.Map<Course>(courseDto);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return _mapper.Map<CourseDto>(course);
        }
        public Task<bool> DeleteCourseAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _context.Courses.ToListAsync();
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }
        public async Task<CourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            return _mapper.Map<CourseDto>(course);
        }
        public async Task<IEnumerable<PrerequisiteDto>> GetPrerequisitesAsync(int courseId)
        {
            return await _context.CoursePrerequisites
                .Where(p => p.CourseId == courseId)
                .Select(p => new PrerequisiteDto
                {
                    CourseId = p.CourseId,
                    PrerequisiteCourseId = p.PrerequisiteCourseId,
                    PrerequisiteCourseName = p.PrerequisiteCourse.Title,
                    IsMandatory = p.IsMandatory
                })
                .ToListAsync();
        }
        public async Task<bool> RemovePrerequisiteAsync(int courseId, int prerequisiteId)
        {
            var prerequisite = await _context.CoursePrerequisites
                .FirstOrDefaultAsync(p => p.CourseId == courseId
                    && p.PrerequisiteCourseId == prerequisiteId);

            if (prerequisite == null)
                return false;

            _context.CoursePrerequisites.Remove(prerequisite);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto courseDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return null;

            _mapper.Map(courseDto, course);
            course.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<CourseDto>(course);
        }

        public async Task UpdateCourseCounterAsync(int courseId, bool increment = true)
        {
            var course = await GetCourse(courseId);
            if (increment)
            {
                course.CurrentEnrollment++;
            }
            else
            {
                course.CurrentEnrollment--;
            }
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

        }

        private async Task<Course> GetCourse(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            return course;
        }
    }
}
