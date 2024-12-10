using LMS.Application.DTOs;

namespace LMS.Application.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto> GetStudentByIdAsync(int id);
        Task<StudentDto> GetStudentByEmailAsync(string email);
        Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto);
        Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto studentDto);
        Task<IEnumerable<EnrollmentDto>> GetStudentEnrollmentsAsync(int studentId);
        Task<bool> DeleteStudentAsync(int id);
        Task<IEnumerable<int>> GetCompletedCourseIdsAsync(int studentId);
    }
}
