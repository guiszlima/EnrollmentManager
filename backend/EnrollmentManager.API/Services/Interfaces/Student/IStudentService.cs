using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Student;

namespace EnrollmentManager.API.Services.Interfaces.Student;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllAsync(StudentFilterDto filter);
    Task<StudentResponseDto?> GetByIdAsync(int userId);
    Task<ApiResponseDto<StudentResponseDto>> CreateAsync(StudentCreateDto dto);
    Task<ApiResponseDto<StudentResponseDto>> UpdateAsync(int userId, StudentUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(int userId);
}
