using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Monolith.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(IEnrollmentService enrollmentService, ICourseService courseService, IStudentService studentService,ILogger<EnrollmentController> logger)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
            _studentService = studentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetAllEnrollments()
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            return Ok(enrollments);
        }
        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> CreateEnrollment(CreateEnrollmentDto enrollmentDto)
        {
            try
            {
                var enrollment = await _enrollmentService.CreateEnrollmentAsync(enrollmentDto);
                await _courseService.UpdateCourseCounterAsync(enrollmentDto.CourseId);

                return Ok(enrollment);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating enrollment");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetStudentEnrollments(int studentId)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(studentId);
            return Ok(enrollments);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetCourseEnrollments(int courseId)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
            return Ok(enrollments);
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelEnrollment(int id)
        {
            var result = await _enrollmentService.CancelEnrollmentAsync(id);
            if (!result) return NotFound();
            else
            {
                await _courseService.UpdateCourseCounterAsync(id, false);
                return NoContent();
            }
            
        }
    }
}
