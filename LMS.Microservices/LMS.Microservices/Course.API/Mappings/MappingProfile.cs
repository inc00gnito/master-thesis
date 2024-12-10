using AutoMapper;
using Course.API.DTOs;

namespace Course.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map CreateCourseDto to Course
            CreateMap<CreateCourseDto, Models.Course>()
                .ForMember(dest => dest.CreatedDate,
                          opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CurrentEnrollment,
                          opt => opt.MapFrom(src => 0));

            // Map Course to CourseDto
            CreateMap<Models.Course, CourseDto>();

            // Map UpdateCourseDto to Course
            CreateMap<UpdateCourseDto, Models.Course>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}