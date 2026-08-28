using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;

namespace EnrollmentManager.API.Services.Interfaces.Enrollment;

public interface IEnrollmentService
{
    Task<ApiResponseDto<EnrollmentResponseDTO>> CreateAsync(EnrollmentCreateDTO dto);
    Task<ApiResponseDto<EnrollmentResponseDTO>> ChangeStatusAsync(int enrollmentId, EnrollmentStatusChangeDto dto);
    Task<ApiResponseDto<EnrollmentResponseDTO>> GetByIdAsync(int enrollmentId);
    Task<ApiResponseDto<List<EnrollmentResponseDTO>>> GetByStudentAsync(int studentId);
}
