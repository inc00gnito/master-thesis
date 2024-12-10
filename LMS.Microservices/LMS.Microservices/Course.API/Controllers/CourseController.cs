using Course.API.DTOs;
using Course.API.Services;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var course = await _courseService.CreateCourseAsync(courseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(int id, UpdateCourseDto courseDto)
        {
            var course = await _courseService.UpdateCourseAsync(id, courseDto);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpGet("{id}/prerequisites")]
        public async Task<ActionResult<IEnumerable<PrerequisiteDto>>> GetPrerequisites(int id)
        {
            var prerequisites = await _courseService.GetPrerequisitesAsync(id);
            if (prerequisites == null)
                return NotFound();

            return Ok(prerequisites);
        }
        [HttpPost("{id}/prerequisites")]
        public async Task<ActionResult<PrerequisiteDto>> AddPrerequisite(int id, AddPrerequisiteDto prerequisiteDto)
        {
            try
            {
                var prerequisite = await _courseService.AddPrerequisiteAsync(id, prerequisiteDto);
                return CreatedAtAction(nameof(GetPrerequisites), new { id }, prerequisite);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (PrerequisiteAlreadyExistsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{courseId}/prerequisites/{prerequisiteId}")]
        public async Task<ActionResult> RemovePrerequisite(int courseId, int prerequisiteId)
        {
            var result = await _courseService.RemovePrerequisiteAsync(courseId, prerequisiteId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
