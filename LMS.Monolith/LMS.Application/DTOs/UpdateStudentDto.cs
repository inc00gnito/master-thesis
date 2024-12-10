using System.ComponentModel.DataAnnotations;

namespace LMS.Application.DTOs
{
    public class UpdateStudentDto
    {
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
