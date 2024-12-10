using Enrollment.API.DTOs;
using System.Collections.Generic;

namespace Enrollment.API.Services;

public interface IEnrollmentService
{
    Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentDto enrollmentDto);
    Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByStudentAsync(int studentId);
    Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByCourseAsync(int courseId);
    Task<EnrollmentDto> GetEnrollmentAsync(int id);
    Task<bool> CancelEnrollmentAsync(int id);
    Task<IEnumerable<EnrollmentDto>> GetAllEnrollmentsAsync();
}
