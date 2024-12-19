using System.ComponentModel.DataAnnotations;

namespace Enrollment.API.DTOs;

public class CreateEnrollmentDto
{
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    public int CourseId { get; set; }
}
