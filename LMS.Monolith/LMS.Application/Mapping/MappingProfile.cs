using AutoMapper;
using LMS.Application.DTOs;
using LMS.Core.Entities;

namespace LMS.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<CreateCourseDto, Course>();

            CreateMap<Student, StudentDto>();
            CreateMap<CreateStudentDto, Student>();

            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(d => d.StudentName,
                    opt => opt.MapFrom(s => $"{s.Student.FirstName} {s.Student.LastName}"))
                .ForMember(d => d.CourseName,
                    opt => opt.MapFrom(s => s.Course.Title));
            CreateMap<Enrollment, EnrollmentDto>();

            CreateMap<CreateEnrollmentDto, Enrollment>()
                .ForMember(dest => dest.EnrollmentDate,
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => EnrollmentStatus.Pending));


        }
        private string MapEnrollmentStatus(EnrollmentStatus status)
        {
            return status switch
            {
                EnrollmentStatus.Pending => "Pending",
                EnrollmentStatus.Completed => "Completed",
                EnrollmentStatus.Active => "Active",
                EnrollmentStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }
    }
}
