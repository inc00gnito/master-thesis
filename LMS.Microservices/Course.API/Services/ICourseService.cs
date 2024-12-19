using Course.API.DTOs;

namespace Course.API.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto);
        Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto courseDto);
        Task<bool> DeleteCourseAsync(int id);
        Task<IEnumerable<PrerequisiteDto>> GetPrerequisitesAsync(int courseId);
        Task<PrerequisiteDto> AddPrerequisiteAsync(int courseId, AddPrerequisiteDto dto);
        Task<bool> RemovePrerequisiteAsync(int courseId, int prerequisiteId);


    }
}