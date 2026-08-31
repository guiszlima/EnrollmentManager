using EnrollmentManager.API.Constants;
using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Interfaces.Enrollment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EnrollmentManager.API.Services.Enrollments;

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(
        ApplicationDbContext context,
        ILogger<EnrollmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> CreateAsync(
        EnrollmentCreateDTO dto,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateCreateRequirementsAsync(
            dto,
            cancellationToken);

        if (validationError != null)
            return Fail(validationError);

        var pendingStatus = await _context.EnrollmentStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(
                status => status.Code == EnrollmentStatusCodes.Pending,
                cancellationToken);

        if (pendingStatus is null)
        {
            _logger.LogError(
                "Status {StatusCode} não está configurado.",
                EnrollmentStatusCodes.Pending);

            return Fail("Status inicial de matrícula não configurado.");
        }

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            FormatId = dto.FormatId,
            StatusId = pendingStatus.Id,
            EnrollmentDate = DateTime.UtcNow,
            IsActiveEnrollment = false
        };

        _context.Enrollments.Add(enrollment);

        return await SaveAndReturnAsync(
            enrollment,
            cancellationToken);
    }

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> ChangeStatusAsync(
        int enrollmentId,
        EnrollmentStatusChangeDto dto,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Status)
            .FirstOrDefaultAsync(
                e => e.Id == enrollmentId,
                cancellationToken);

        if (enrollment is null)
            return Fail("Matrícula não encontrada.");

        var targetStatus = await _context.EnrollmentStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(
                status => status.Id == dto.StatusId,
                cancellationToken);

        if (targetStatus is null)
            return Fail("Status de matrícula não encontrado.");

        if (!IsTransitionAllowed(
                enrollment.Status.Code,
                targetStatus.Code))
        {
            return Fail("Transição de status não permitida.");
        }

        if (targetStatus.Code == EnrollmentStatusCodes.Approved &&
            await HasActiveEnrollmentAsync(
                enrollment,
                cancellationToken))
        {
            return Fail(
                "Já existe uma matrícula ativa para este aluno, curso e modalidade.");
        }

        enrollment.ApplyStatusChange(
            targetStatus,
            EnrollmentStatusCodes.Approved,
            EnrollmentStatusCodes.Completed);

        return await SaveAndReturnAsync(
            enrollment,
            cancellationToken);
    }

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> GetByIdAsync(
        int enrollmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollment = await Project()
                .FirstOrDefaultAsync(
                    e => e.Id == enrollmentId,
                    cancellationToken);

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

            return Fail<List<EnrollmentResponseDTO>>("Não foi possível consultar as matrículas.");
        }
    }

    private async Task<string?> ValidateCreateRequirementsAsync(
        EnrollmentCreateDTO dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validation = await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId == dto.StudentId)
                .Select(s => new
                {
                    IsStudentActive = s.User.IsActive,

                    CourseStatusCode = _context.Courses
                        .Where(c => c.Id == dto.CourseId)
                        .Select(c => c.CourseStatus.Code)
                        .FirstOrDefault(),

                    IsFormatAllowed = _context.CourseStudyFormats
                        .Any(csf =>
                            csf.CourseId == dto.CourseId &&
                            csf.FormatId == dto.FormatId),

                    HasExistingEnrollment = _context.Enrollments
                        .Any(e =>
                            e.StudentId == dto.StudentId &&
                            e.CourseId == dto.CourseId &&
                            e.FormatId == dto.FormatId &&
                            (e.IsActiveEnrollment ||
                             e.Status.Code == EnrollmentStatusCodes.Pending))
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (validation is null)
                return "Aluno não encontrado.";

            if (!validation.IsStudentActive)
                return "Usuário do aluno está inativo.";

            if (validation.CourseStatusCode is null)
                return "Curso não encontrado.";

            if (validation.CourseStatusCode != CourseStatusCodes.Active)
                return "Curso não está disponível para novas matrículas.";

            if (!validation.IsFormatAllowed)
                return "A modalidade informada não é aceita por este curso ou não existe.";

            if (validation.HasExistingEnrollment)
                return "Você já possui uma matrícula ativa ou pendente para este curso e modalidade.";

            return null;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao validar criação de matrícula para StudentId={StudentId}, CourseId={CourseId}, FormatId={FormatId}.",
                dto.StudentId,
                dto.CourseId,
                dto.FormatId);

            return "Não foi possível validar os requisitos da matrícula.";
        }
    }

    private async Task<bool> HasActiveEnrollmentAsync(
        Enrollment current,
        CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .AnyAsync(
                e =>
                    e.Id != current.Id &&
                    e.StudentId == current.StudentId &&
                    e.CourseId == current.CourseId &&
                    e.FormatId == current.FormatId &&
                    e.IsActiveEnrollment,
                cancellationToken);
    }

    private async Task<ApiResponseDto<EnrollmentResponseDTO>> SaveAndReturnAsync(
        Enrollment enrollment,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(
                enrollment.Id,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(
                ex,
                "Conflito de concorrência ao atualizar matrícula {EnrollmentId}.",
                enrollment.Id);

            return Fail(
                "A matrícula foi alterada por outra operação. Tente novamente.");
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            _logger.LogInformation(
                ex,
                "Conflito de unicidade ao salvar matrícula. " +
                "StudentId={StudentId}, CourseId={CourseId}, FormatId={FormatId}.",
                enrollment.StudentId,
                enrollment.CourseId,
                enrollment.FormatId);

            return Fail(
                "Já existe uma matrícula ativa para este aluno, curso e modalidade.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Erro de persistência ao salvar matrícula {EnrollmentId}.",
                enrollment.Id);

            return Fail(
                "Não foi possível salvar a matrícula.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro inesperado ao salvar matrícula {EnrollmentId}.",
                enrollment.Id);

            return Fail(
                "Ocorreu um erro inesperado ao processar a matrícula.");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException &&
               postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }

 
    private static ApiResponseDto<EnrollmentResponseDTO> Fail(string message) =>
        ApiResponseDto<EnrollmentResponseDTO>.Error(message);

    private static ApiResponseDto<T> Fail<T>(string message) =>
        ApiResponseDto<T>.Error(message);

    private static ApiResponseDto<T> Success<T>(T data) =>
        new() { Data = data };

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

    private static bool IsTransitionAllowed(
        string current,
        string target) => (current, target) switch
    {
        (
            EnrollmentStatusCodes.Pending,
            EnrollmentStatusCodes.Approved or EnrollmentStatusCodes.Cancelled
        ) => true,

        (
            EnrollmentStatusCodes.Approved,
            EnrollmentStatusCodes.Suspended
                or EnrollmentStatusCodes.Completed
                or EnrollmentStatusCodes.Cancelled
        ) => true,

        (
            EnrollmentStatusCodes.Suspended,
            EnrollmentStatusCodes.Approved or EnrollmentStatusCodes.Cancelled
        ) => true,

        _ => false
    };
}