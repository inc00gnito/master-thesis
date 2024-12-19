namespace Student.API.Models;

public class CompletedCourse
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime CompletionDate { get; set; }
    public decimal Grade { get; set; }

    // Navigation properties
    public Student Student { get; set; }
}