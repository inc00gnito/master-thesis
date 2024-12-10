namespace LMS.Core.Entities
{
    // LMS.Core/Entities/CoursePrerequisite.cs
    public class CoursePrerequisite
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int PrerequisiteCourseId { get; set; }
        public bool IsMandatory { get; set; }
        public Course Course { get; set; }
        public Course PrerequisiteCourse { get; set; }
    }
}
