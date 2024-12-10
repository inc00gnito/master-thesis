using Microsoft.AspNetCore.Mvc;
using Student.API.DTOs;
using Student.API.Models;
using Student.API.Services;

namespace Student.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var student = await _studentService.CreateStudentAsync(studentDto);
            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentDto>> UpdateStudent(int id, UpdateStudentDto studentDto)
        {
            var student = await _studentService.UpdateStudentAsync(id, studentDto);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpGet("{id}/completed-courses")]
        public async Task<ActionResult<IEnumerable<int>>> GetCompletedCourseIds(int id)
        {
            var courseIds = await _studentService.GetCompletedCourseIdsAsync(id);
            return Ok(courseIds);
        }
        [HttpGet("{id}/enrollments")]
        public async Task<ActionResult<IEnumerable<StudentEnrollment>>> GetStudentEnrollments(int id)
        {
            var courseIds = await _studentService.GetStudentEnrollments(id);
            return Ok(courseIds);
        }

        //[HttpGet("{id}/completed-courses/details")]
        //public async Task<ActionResult<IEnumerable<IEnumerable<int>>>> GetCompletedCourses(int id)
        //{
        //    var courses = await _studentService.GetCompletedCoursesAsync(id);
        //    return Ok(courses);
        //}

        //[HttpPost("{id}/completed-courses")]
        //public async Task<ActionResult<CompletedCourseDto>> AddCompletedCourse(
        //    int id, CompletedCourseDto courseDto)
        //{
        //    var completedCourse = await _studentService.AddCompletedCourseAsync(id, courseDto);
        //    return CreatedAtAction(
        //        nameof(GetCompletedCourses),
        //        new { id },
        //        completedCourse);
        //}
    }
}