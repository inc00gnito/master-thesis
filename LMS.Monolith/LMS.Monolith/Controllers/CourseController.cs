using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Monolith.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(ICourseService courseService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto courseDto)
        {
            try
            {
                var course = await _courseService.CreateCourseAsync(courseDto);
                return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/prerequisites")]
        public async Task<ActionResult<IEnumerable<PrerequisiteDto>>> GetPrerequisites(int id)
        {
            var prerequisites = await _courseService.GetPrerequisitesAsync(id);
            return Ok(prerequisites);
        }

        [HttpPost("{id}/prerequisites")]
        public async Task<ActionResult<PrerequisiteDto>> AddPrerequisite(int id, AddPrerequisiteDto prerequisiteDto)
        {
            try
            {
                var prerequisite = await _courseService.AddPrerequisiteAsync(id, prerequisiteDto);
                return Ok(prerequisite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding prerequisite");
                return BadRequest(ex.Message);
            }
        }
    }
}
