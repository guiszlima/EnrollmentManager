using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnrollmentManager.API.Services.Enrollments;

public class EnrollmentQueryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EnrollmentQueryService> _logger;

    public EnrollmentQueryService(
        ApplicationDbContext context,
        ILogger<EnrollmentQueryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> GetByIdAsync(
        int enrollmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollment = await Project()
                .FirstOrDefaultAsync(e => e.Id == enrollmentId, cancellationToken);

            return enrollment is null
                ? Fail("Matrícula não encontrada.")
                : Success(enrollment);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao buscar matrícula {EnrollmentId}.",
                enrollmentId);

            return Fail("Não foi possível consultar a matrícula.");
        }
    }

    public async Task<ApiResponseDto<List<EnrollmentResponseDTO>>> GetByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollments = await Project()
                .Where(e => e.StudentId == studentId)
                .ToListAsync(cancellationToken);

            return Success(enrollments);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao buscar matrículas do aluno {StudentId}.",
                studentId);

            return Fail<List<EnrollmentResponseDTO>>(
                "Não foi possível consultar as matrículas.");
        }
    }

    public async Task<ApiResponseDto<List<EnrollmentResponseDTO>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollments = await Project().ToListAsync(cancellationToken);
            return Success(enrollments);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todas as matrículas.");
            return Fail<List<EnrollmentResponseDTO>>(
                "Não foi possível consultar as matrículas.");
        }
    }

    private IQueryable<EnrollmentResponseDTO> Project() =>
        _context.Enrollments
            .AsNoTracking()
            .Select(enrollment => new EnrollmentResponseDTO
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = enrollment.Student.User.UserName,
                CourseId = enrollment.CourseId,
                CourseName = enrollment.Course.Name,
                StatusId = enrollment.StatusId,
                StatusName = enrollment.Status.Name,
                FormatId = enrollment.FormatId,
                FormatName = enrollment.Format.Name,
                EnrollmentDate = enrollment.EnrollmentDate,
                CompletionDate = enrollment.CompletionDate
            });

    private static ApiResponseDto<EnrollmentResponseDTO> Fail(string message) =>
        ApiResponseDto<EnrollmentResponseDTO>.Error(message);

    private static ApiResponseDto<T> Fail<T>(string message) =>
        ApiResponseDto<T>.Error(message);

    private static ApiResponseDto<T> Success<T>(T data) =>
        new() { Data = data };
}
