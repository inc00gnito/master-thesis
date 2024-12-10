using Enrollment.API.Models;

namespace Enrollment.API.DTOs;
public class EnrollmentDetailDto
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
}