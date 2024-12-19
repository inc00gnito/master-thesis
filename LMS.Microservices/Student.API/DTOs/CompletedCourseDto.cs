namespace Student.API.DTOs
{
    public class CompletedCourseDto
    {
        public int CourseId { get; set; }
        public DateTime CompletionDate { get; set; }
        public decimal Grade { get; set; }
    }
}