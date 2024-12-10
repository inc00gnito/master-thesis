using System.ComponentModel.DataAnnotations;

namespace LMS.Application.DTOs
{
    public class UpdateCourseDto
    {
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 1000)]
        public int? MaxEnrollment { get; set; }
    }
}
