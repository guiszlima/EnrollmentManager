using EnrollmentManager.API.Constants;
using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;
using EnrollmentManager.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnrollmentManager.API.Services.Enrollments;

public class EnrollmentStatusService
{
    private readonly ApplicationDbContext _context;
    private readonly EnrollmentQueryService _queryService;
    private readonly ILogger<EnrollmentStatusService> _logger;

    public EnrollmentStatusService(
        ApplicationDbContext context,
        EnrollmentQueryService queryService,
        ILogger<EnrollmentStatusService> logger)
    {
        _context = context;
        _queryService = queryService;
        _logger = logger;
    }

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> ChangeStatusAsync(
        int enrollmentId,
        EnrollmentStatusChangeDto dto,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Status)
            .Include(e => e.Student)
                .ThenInclude(student => student.User)
                    .ThenInclude(user => user.Role)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId, cancellationToken);

        if (enrollment is null)
            return Fail("Matrícula não encontrada.");

        var targetStatus = await _context.EnrollmentStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(status => status.Id == dto.StatusId, cancellationToken);

        if (targetStatus is null)
            return Fail("Status de matrícula não encontrado.");

        if (!IsTransitionAllowed(enrollment.Status.Code, targetStatus.Code))
            return Fail("Transição de status não permitida.");

        if (targetStatus.Code == EnrollmentStatusCodes.Approved &&
            !enrollment.Student.User.IsActive)
        {
            return Fail("Usuário do aluno está inativo.");
        }

        if (targetStatus.Code == EnrollmentStatusCodes.Approved &&
            enrollment.Student.User.Role?.Code != "STUDENT")
        {
            return Fail("O usuário não possui o cargo de aluno.");
        }

        if (targetStatus.Code == EnrollmentStatusCodes.Approved &&
            await HasActiveEnrollmentAsync(enrollment, cancellationToken))
        {
            return Fail("Já existe uma matrícula ativa para este aluno e curso.");
        }

        var releasesSeat = targetStatus.Code is EnrollmentStatusCodes.Cancelled or EnrollmentStatusCodes.Completed;
        var consumesSeatAgain = targetStatus.Code == EnrollmentStatusCodes.Approved && !enrollment.ConsumesSeat;

        if (consumesSeatAgain)
        {
            var course = await _context.Courses.FirstAsync(current => current.Id == enrollment.CourseId, cancellationToken);
            if (course.AvailableSlots <= 0)
                return Fail("Curso sem vagas disponíveis.");

            course.AvailableSlots--;
        }

        if (releasesSeat && enrollment.ConsumesSeat)
        {
            var course = await _context.Courses.FirstAsync(current => current.Id == enrollment.CourseId, cancellationToken);
            course.AvailableSlots = Math.Min(course.TotalSlots, course.AvailableSlots + 1);
        }

        enrollment.ApplyStatusChange(
            targetStatus,
            EnrollmentStatusCodes.Approved,
            EnrollmentStatusCodes.Completed);
        enrollment.ConsumesSeat = !releasesSeat;

        return await SaveAndReturnAsync(enrollment, cancellationToken);
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
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Erro de persistência ao atualizar matrícula {EnrollmentId}.",
                enrollment.Id);

            return Fail("Não foi possível salvar a matrícula.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro inesperado ao atualizar matrícula {EnrollmentId}.",
                enrollment.Id);

            return Fail("Ocorreu um erro inesperado ao processar a matrícula.");
        }
    }

    private static bool IsTransitionAllowed(string current, string target) => (current, target) switch
    {
        (EnrollmentStatusCodes.Pending, EnrollmentStatusCodes.Approved or EnrollmentStatusCodes.Cancelled) => true,
        (EnrollmentStatusCodes.Approved, EnrollmentStatusCodes.Suspended or EnrollmentStatusCodes.Completed or EnrollmentStatusCodes.Cancelled) => true,
        (EnrollmentStatusCodes.Suspended, EnrollmentStatusCodes.Approved or EnrollmentStatusCodes.Cancelled) => true,
        _ => false
    };

    private static ApiResponseDto<EnrollmentResponseDTO> Fail(string message) =>
        ApiResponseDto<EnrollmentResponseDTO>.Error(message);
}
