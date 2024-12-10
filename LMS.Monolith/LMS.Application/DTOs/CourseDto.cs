namespace LMS.Application.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxEnrollment { get; set; }
        public int CurrentEnrollment { get; set; }
    }
}
