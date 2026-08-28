using EnrollmentManager.API.DTOs.Catalogs;

namespace EnrollmentManager.API.Services.Interfaces.Catalogs;

public interface IEducationLevelService
{
    Task<List<EducationLevelResponseDto>> GetAllAsync();
    Task<EducationLevelResponseDto?> GetByIdAsync(int id);
}