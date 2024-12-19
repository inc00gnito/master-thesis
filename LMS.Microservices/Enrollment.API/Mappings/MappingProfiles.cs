using AutoMapper;
using Enrollment.API.DTOs;
using Enrollment.API.Models;

namespace Enrollment.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Enrollment, EnrollmentDto>();

            CreateMap<CreateEnrollmentDto, Models.Enrollment>()
                .ForMember(dest => dest.EnrollmentDate, 
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, 
                    opt => opt.MapFrom(src => EnrollmentStatus.Pending));

            CreateMap<Models.Enrollment, EnrollmentDetailDto>()
                .ForMember(dest => dest.EnrollmentId, 
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapEnrollmentStatus(src.Status)));
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
