using LMS.Application.DTOs;

namespace LMS.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentDto enrollmentDto);
        Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByStudentAsync(int studentId);
        Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByCourseAsync(int courseId);
        Task<EnrollmentDto> GetEnrollmentAsync(int id);
        Task<bool> CancelEnrollmentAsync(int id);
        Task<IEnumerable<EnrollmentDto>> GetAllEnrollmentsAsync();
    }
}
