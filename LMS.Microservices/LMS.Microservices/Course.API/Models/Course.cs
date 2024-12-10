namespace Course.API.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxEnrollment { get; set; }
        public int CurrentEnrollment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

         public ICollection<CoursePrerequisite> Prerequisites { get; set; }
    }

}