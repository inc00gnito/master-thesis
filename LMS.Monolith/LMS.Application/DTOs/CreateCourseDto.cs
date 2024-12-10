namespace LMS.Application.DTOs
{
    public class CreateCourseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxEnrollment { get; set; }
    }
}
