using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;

namespace EnrollmentManager.API.Services.Interfaces.Enrollment;

public interface IEnrollmentService
{
    Task<ApiResponseDto<EnrollmentResponseDTO>> CreateAsync(
        EnrollmentCreateDTO dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<EnrollmentResponseDTO>> ChangeStatusAsync(
        int enrollmentId,
        EnrollmentStatusChangeDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<EnrollmentResponseDTO>> GetByIdAsync(
        int enrollmentId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<List<EnrollmentResponseDTO>>> GetByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default);
}