using AutoMapper;
using Course.API.Data;
using Course.API.DTOs;
using Course.API.Models;
using EventBus.Messages.Events;
using Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Course.API.Services
{
    public class CourseService : ICourseService
    {
        private readonly CourseContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CourseService> _logger;

        public CourseService(
            CourseContext context,
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<CourseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
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

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto)
        {
            var course = _mapper.Map<Course.API.Models.Course>(courseDto);
            course.CreatedDate = DateTime.UtcNow;
            course.CurrentEnrollment = 0;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Publish course created event
            await _publishEndpoint.Publish(new CourseCreatedEvent
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                MaxEnrollment = course.MaxEnrollment
            });

            return _mapper.Map<CourseDto>(course);
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

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<PrerequisiteDto>> GetPrerequisitesAsync(int courseId)
        {
            return await _context.CoursePrerequisites
                .Where(p => p.CourseId == courseId)
                .Select(p => new PrerequisiteDto
                {
                    CourseId = p.CourseId,
                    PrerequisiteCourseId = p.PrerequisiteCourseId,
                    IsMandatory = p.IsMandatory
                })
                .ToListAsync();
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

            // Check for circular dependency
            if (await HasCircularDependency(courseId, dto.PrerequisiteCourseId))
                throw new InvalidOperationException("Adding this prerequisite would create a circular dependency");

            var prerequisite = new CoursePrerequisite
            {
                CourseId = courseId,
                PrerequisiteCourseId = dto.PrerequisiteCourseId,
                IsMandatory = dto.IsMandatory,
                PrerequisiteCourseName = dto.PrerequisiteCourseName,
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

        private async Task<bool> HasCircularDependency(int courseId, int newPrerequisiteId)
        {
            var visited = new HashSet<int>();
            var stack = new Stack<int>();
            stack.Push(newPrerequisiteId);

            while (stack.Count > 0)
            {
                var currentId = stack.Pop();
                if (!visited.Add(currentId))
                    continue;

                if (currentId == courseId)
                    return true;

                var prerequisites = await _context.CoursePrerequisites
                    .Where(p => p.CourseId == currentId)
                    .Select(p => p.PrerequisiteCourseId)
                    .ToListAsync();

                foreach (var prerequisiteId in prerequisites)
                {
                    stack.Push(prerequisiteId);
                }
            }

            return false;
        }
    }
}