using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;
using EnrollmentManager.API.Services.Interfaces.Enrollment;

namespace EnrollmentManager.API.Services.Enrollments;

public class EnrollmentService : IEnrollmentService
{
    private readonly EnrollmentCreationService _creationService;
    private readonly EnrollmentStatusService _statusService;
    private readonly EnrollmentQueryService _queryService;

    public EnrollmentService(
        EnrollmentCreationService creationService,
        EnrollmentStatusService statusService,
        EnrollmentQueryService queryService)
    {
        _creationService = creationService;
        _statusService = statusService;
        _queryService = queryService;
    }

    public Task<ApiResponseDto<EnrollmentResponseDTO>> CreateAsync(
        EnrollmentCreateDTO dto,
        CancellationToken cancellationToken = default) =>
        _creationService.CreateAsync(dto, cancellationToken);

    public Task<ApiResponseDto<EnrollmentResponseDTO>> ChangeStatusAsync(
        int enrollmentId,
        EnrollmentStatusChangeDto dto,
        CancellationToken cancellationToken = default) =>
        _statusService.ChangeStatusAsync(enrollmentId, dto, cancellationToken);

    public Task<ApiResponseDto<EnrollmentResponseDTO>> GetByIdAsync(
        int enrollmentId,
        CancellationToken cancellationToken = default) =>
        _queryService.GetByIdAsync(enrollmentId, cancellationToken);

    public Task<ApiResponseDto<List<EnrollmentResponseDTO>>> GetByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default) =>
        _queryService.GetByStudentAsync(studentId, cancellationToken);

    public Task<ApiResponseDto<List<EnrollmentResponseDTO>>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        _queryService.GetAllAsync(cancellationToken);
}
