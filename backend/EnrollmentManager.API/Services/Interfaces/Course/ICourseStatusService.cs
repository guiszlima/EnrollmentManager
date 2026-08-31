using EnrollmentManager.API.DTOs.Catalogs;

namespace EnrollmentManager.API.Services.Interfaces.Course;

public interface ICourseStatusService
{
    Task<List<CourseStatusResponseDto>> GetAllAsync();

    Task<CourseStatusResponseDto?> GetByIdAsync(int id);

}