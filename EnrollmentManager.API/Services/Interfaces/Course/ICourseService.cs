using EnrollmentManager.API.DTOs.Course;

namespace EnrollmentManager.API.Services.Interfaces.Course;

public interface ICourseService
{
    Task<List<CourseResponseDto>> GetAllAsync();
    Task<CourseResponseDto?> GetByIdAsync(int id);
    Task<CourseResponseDto?> CreateAsync(CourseInputDto dto);
    Task<CourseResponseDto?> UpdateAsync(int id, CourseInputDto dto);
    Task<bool> DeleteAsync(int id);
}
