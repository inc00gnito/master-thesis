using Student.API.DTOs;
using Student.API.Models;

namespace Student.API.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto> GetStudentByIdAsync(int id);
        Task<StudentDto> GetStudentByEmailAsync(string email);
        Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto);
        Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto studentDto);
        Task<IEnumerable<StudentEnrollment>> GetStudentEnrollments(int studentId);
        Task<bool> DeleteStudentAsync(int id);
        Task<IEnumerable<int>> GetCompletedCourseIdsAsync(int studentId);
        //Task<IEnumerable<CompletedCourseDto>> GetCompletedCoursesAsync(int studentId);
        //Task<CompletedCourseDto> AddCompletedCourseAsync(int studentId, CompletedCourseDto courseDto);

    }
}

