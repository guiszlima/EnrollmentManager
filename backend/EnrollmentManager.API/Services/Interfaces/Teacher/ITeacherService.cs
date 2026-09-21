using EnrollmentManager.API.DTOs.Teacher;

namespace EnrollmentManager.API.Services.Interfaces.Teacher;

public interface ITeacherService
{
    Task<List<TeacherResponseDto>> GetAllAsync(TeacherFilterDto filter);
    Task<TeacherResponseDto?> GetByIdAsync(int userId);
    Task<EnrollmentManager.API.DTOs.Common.ApiResponseDto<TeacherResponseDto>> CreateAsync(
        int userId,
        IEnumerable<int> formatIds);
    Task<EnrollmentManager.API.DTOs.Common.ApiResponseDto<TeacherResponseDto>> UpdateAsync(
        int userId,
        IEnumerable<int> formatIds);
}
