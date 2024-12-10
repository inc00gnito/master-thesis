using LMS.Application.DTOs;

namespace LMS.Application.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto);
        Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto courseDto);
        Task UpdateCourseCounterAsync(int courseId, bool increment = true);
        Task<bool> DeleteCourseAsync(int id);
        Task<IEnumerable<PrerequisiteDto>> GetPrerequisitesAsync(int courseId);
        Task<PrerequisiteDto> AddPrerequisiteAsync(int courseId, AddPrerequisiteDto prerequisiteDto);
        Task<bool> RemovePrerequisiteAsync(int courseId, int prerequisiteId);
    }
}
