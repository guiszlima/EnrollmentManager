using EnrollmentManager.API.DTOs.Catalogs;

namespace EnrollmentManager.API.Services.Interfaces.Catalogs;

public interface ICourseStudyFormatService
{
    Task<List<CourseStudyFormatResponseDto>> GetAllAsync();
    Task<CourseStudyFormatResponseDto?> GetByIdAsync(int courseId, int formatId);
}