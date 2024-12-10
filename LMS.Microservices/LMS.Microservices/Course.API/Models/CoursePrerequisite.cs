namespace Course.API.Models
{
    public class CoursePrerequisite
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int PrerequisiteCourseId { get; set; }
        public bool IsMandatory { get; set; }
        public string PrerequisiteCourseName { get; set; }

        // Navigation property for the main course
        //public Course Course { get; set; }
    }
}