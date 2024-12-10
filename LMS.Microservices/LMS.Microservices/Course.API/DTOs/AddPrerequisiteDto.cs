namespace Course.API.DTOs
{
    public class AddPrerequisiteDto
    {
        public int PrerequisiteCourseId { get; set; }
        public string PrerequisiteCourseName { get; set; }
        public bool IsMandatory { get; set; }
    }
}