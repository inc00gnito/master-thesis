using System.ComponentModel.DataAnnotations;

namespace Student.API.DTOs
{
    public class CreateStudentDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}