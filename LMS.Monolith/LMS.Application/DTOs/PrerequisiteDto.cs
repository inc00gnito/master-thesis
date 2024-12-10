namespace LMS.Application.DTOs
{
    public class PrerequisiteDto
    {
        public int CourseId { get; set; }
        public int PrerequisiteCourseId { get; set; }
        public string PrerequisiteCourseName { get; set; }
        public bool IsMandatory { get; set; }
    }
}
