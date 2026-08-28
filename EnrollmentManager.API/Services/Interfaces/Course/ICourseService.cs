using EnrollmentManager.API.DTOs.Course;

namespace EnrollmentManager.API.Services.Interfaces.Course;

public interface ICourseService
{
    Task<List<CourseResponseDTO>> GetAllAsync();
    Task<CourseResponseDTO?> GetByIdAsync(int id);
    Task<CourseResponseDTO?> CreateAsync(CourseInputDTO dto);
    Task<CourseResponseDTO?> UpdateAsync(int id, CourseInputDTO dto);
    Task<bool> DeleteAsync(int id);
}
