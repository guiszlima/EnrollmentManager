using EnrollmentManager.API.DTOs.Catalogs;

namespace EnrollmentManager.API.Services.Interfaces.Catalogs;

public interface ICourseTypeService
{
    Task<List<CourseTypeResponseDto>> GetAllAsync();
    Task<CourseTypeResponseDto?> GetByIdAsync(int id);
}