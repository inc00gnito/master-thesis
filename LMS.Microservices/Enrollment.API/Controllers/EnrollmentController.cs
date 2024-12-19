using Enrollment.API.DTOs;
using Enrollment.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Enrollment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(IEnrollmentService enrollmentService, ILogger<EnrollmentController> logger)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<EnrollmentDto>> GetAllEnrollments()
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
                return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollment);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollment(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentAsync(id);
            if (enrollment == null) return NotFound();
            return Ok(enrollment);
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelEnrollment(int id)
        {
            var result = await _enrollmentService.CancelEnrollmentAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}