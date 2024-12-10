using LMS.Application.Interfaces;
using LMS.Application.Mapping;
using LMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();

            return services;
        }
    }
}
