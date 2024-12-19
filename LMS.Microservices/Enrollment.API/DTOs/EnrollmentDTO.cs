using Enrollment.API.Models;

namespace Enrollment.API.DTOs;
public class EnrollmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; }
}
