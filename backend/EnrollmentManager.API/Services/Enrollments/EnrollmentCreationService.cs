using EnrollmentManager.API.Constants;
using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;
using EnrollmentManager.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EnrollmentManager.API.Services.Enrollments;

public class EnrollmentCreationService
{
    private readonly ApplicationDbContext _context;
    private readonly EnrollmentQueryService _queryService;
    private readonly ILogger<EnrollmentCreationService> _logger;

    public EnrollmentCreationService(
        ApplicationDbContext context,
        EnrollmentQueryService queryService,
        ILogger<EnrollmentCreationService> logger)
    {
        _context = context;
        _queryService = queryService;
        _logger = logger;
    }

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> CreateAsync(
        EnrollmentCreateDTO dto,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateCreateRequirementsAsync(dto, cancellationToken);

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

        IDbContextTransaction? transaction = null;
        int updatedCourses;
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            var course = await _context.Courses.FirstOrDefaultAsync(course => course.Id == dto.CourseId, cancellationToken);
            if (course is null || course.AvailableSlots <= 0)
                return Fail("Curso sem vagas disponíveis.");

            course.AvailableSlots--;
            updatedCourses = 1;
        }
        else
        {
            transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            updatedCourses = await _context.Courses
                .Where(course => course.Id == dto.CourseId && course.AvailableSlots > 0)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(course => course.AvailableSlots, course => course.AvailableSlots - 1),
                    cancellationToken);
        }

        if (updatedCourses == 0)
            return Fail("Curso sem vagas disponíveis.");

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            FormatId = dto.FormatId,
            StatusId = pendingStatus.Id,
            EnrollmentDate = DateTime.UtcNow,
            IsActiveEnrollment = false,
            ConsumesSeat = true
        };

        _context.Enrollments.Add(enrollment);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
                await transaction.CommitAsync(cancellationToken);
            return await _queryService.GetByIdAsync(enrollment.Id, cancellationToken);
        }
        catch (Exception exception) when (exception is DbUpdateException or DbUpdateConcurrencyException)
        {
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(exception, "Erro ao salvar matrícula {StudentId}/{CourseId}.", dto.StudentId, dto.CourseId);
            return Fail("Não foi possível salvar a matrícula.");
        }
        finally
        {
            if (transaction is not null)
                await transaction.DisposeAsync();
        }
    }

    private async Task<EnrollmentCreateValidation?> GetCreateValidationAsync(
        EnrollmentCreateDTO dto,
        CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.UserId == dto.StudentId)
            .Select(s => new EnrollmentCreateValidation
            {
                IsStudentActive = s.User.IsActive,
                CourseStatusCode = _context.Courses
                    .Where(c => c.Id == dto.CourseId)
                    .Select(c => c.CourseStatus.Code)
                    .FirstOrDefault(),
                AvailableSlots = _context.Courses
                    .Where(c => c.Id == dto.CourseId)
                    .Select(c => c.AvailableSlots)
                    .FirstOrDefault(),
                IsFormatAllowed = _context.CourseStudyFormats
                    .Any(csf =>
                        csf.CourseId == dto.CourseId &&
                        csf.FormatId == dto.FormatId),
                IsStudentFormatAllowed = _context.StudentStudyFormats
                    .Any(ssf =>
                        ssf.StudentId == dto.StudentId &&
                        ssf.FormatId == dto.FormatId),
                HasExistingEnrollment = _context.Enrollments
                    .Any(e =>
                        e.StudentId == dto.StudentId &&
                        e.CourseId == dto.CourseId &&
                        (e.IsActiveEnrollment ||
                         e.Status.Code == EnrollmentStatusCodes.Pending))
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<string?> ValidateCreateRequirementsAsync(
        EnrollmentCreateDTO dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validation = await GetCreateValidationAsync(dto, cancellationToken);

            if (validation is null)
                return "Aluno não encontrado.";

            if (!validation.IsStudentActive)
                return "Usuário do aluno está inativo.";

            if (validation.CourseStatusCode is null)
                return "Curso não encontrado.";

            if (validation.CourseStatusCode != CourseStatusCodes.Active)
                return "Curso não está disponível para novas matrículas.";

            if (validation.AvailableSlots <= 0)
                return "Curso sem vagas disponíveis.";

            if (!validation.IsStudentFormatAllowed)
                return "Não tem formatos disponiveis";

            if (!validation.IsFormatAllowed)
                return "Não tem formatos disponiveis";

            if (validation.HasExistingEnrollment)
                return "Você já possui uma matrícula ativa ou pendente para este curso.";

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

    private async Task<ApiResponseDto<EnrollmentResponseDTO>> SaveAndReturnAsync(
        Enrollment enrollment,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return await _queryService.GetByIdAsync(enrollment.Id, cancellationToken);
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

            return Fail("A matrícula foi alterada por outra operação. Tente novamente.");
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

            return Fail("Já existe uma matrícula ativa para este aluno e curso.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Erro de persistência ao salvar matrícula {EnrollmentId}.",
                enrollment.Id);

            return Fail("Não foi possível salvar a matrícula.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro inesperado ao salvar matrícula {EnrollmentId}.",
                enrollment.Id);

            return Fail("Ocorreu um erro inesperado ao processar a matrícula.");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException postgresException &&
        postgresException.SqlState == PostgresErrorCodes.UniqueViolation;

    private static ApiResponseDto<EnrollmentResponseDTO> Fail(string message) =>
        ApiResponseDto<EnrollmentResponseDTO>.Error(message);

    private sealed class EnrollmentCreateValidation
    {
        public bool IsStudentActive { get; init; }
        public string? CourseStatusCode { get; init; }
        public bool IsFormatAllowed { get; init; }
        public bool IsStudentFormatAllowed { get; init; }
        public int AvailableSlots { get; init; }
        public bool HasExistingEnrollment { get; init; }
    }
}
