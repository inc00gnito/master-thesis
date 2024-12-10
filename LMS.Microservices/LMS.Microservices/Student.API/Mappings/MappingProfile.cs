// Mappings/MappingProfile.cs
using AutoMapper;
using Student.API.DTOs;

namespace Student.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Student, StudentDto>();
            CreateMap<CreateStudentDto, Models.Student>()
                .ForMember(dest => dest.CreatedDate,
                          opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateStudentDto, Models.Student>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}