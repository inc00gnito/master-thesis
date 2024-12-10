using System.ComponentModel.DataAnnotations;

namespace Course.API.DTOs
{
    public class CreateCourseDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000)]
        public int MaxEnrollment { get; set; }
    }
}