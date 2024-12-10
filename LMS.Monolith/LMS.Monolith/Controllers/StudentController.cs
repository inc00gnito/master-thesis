using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Monolith.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService studentService, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent(CreateStudentDto studentDto)
        {
            try
            {
                var student = await _studentService.CreateStudentAsync(studentDto);
                return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/enrollments")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetStudentEnrollments(int id)
        {
            var enrollments = await _studentService.GetStudentEnrollmentsAsync(id);
            return Ok(enrollments);
        }
    }
}
