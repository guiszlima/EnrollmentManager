using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Interfaces.Enrollment;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Enrollments;

public class EnrollmentService : IEnrollmentService
{
    private const string Pending = "PENDING";
    private const string Approved = "APPROVED";
    private const string Suspended = "SUSPENDED";
    private const string Cancelled = "CANCELLED";
    private const string Completed = "COMPLETED";

    private readonly ApplicationDbContext _context;

    public EnrollmentService(ApplicationDbContext context) => _context = context;

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> CreateAsync(EnrollmentCreateDTO dto)
    {
        var student = await _context.Students
            .Include(current => current.User)
            .SingleOrDefaultAsync(current => current.UserId == dto.StudentId);
        if (student is null)
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Aluno não encontrado.");
        if (!student.User.IsActive)
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Usuário do aluno está inativo.");

        var course = await _context.Courses
            .Include(current => current.CourseStatus)
            .SingleOrDefaultAsync(current => current.Id == dto.CourseId);
        if (course is null)
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Curso não encontrado.");
        if (course.CourseStatus.Code != "ACTIVE")
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Curso não está disponível para novas matrículas.");

        if (!await _context.StudyFormats.AnyAsync(format => format.Id == dto.FormatId))
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Modalidade não encontrada.");
        if (!await _context.CourseStudyFormats.AnyAsync(pair => pair.CourseId == dto.CourseId && pair.FormatId == dto.FormatId))
            return ApiResponseDto<EnrollmentResponseDTO>.Error("A modalidade não é aceita pelo curso.");

        var pendingStatus = await _context.EnrollmentStatuses.SingleOrDefaultAsync(status => status.Code == Pending);
        if (pendingStatus is null)
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Status inicial de matrícula não configurado.");

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            FormatId = dto.FormatId,
            StatusId = pendingStatus.Id,
            EnrollmentDate = DateTime.UtcNow,
            CompletionDate = null,
            IsActiveEnrollment = false
        };
        _context.Enrollments.Add(enrollment);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Já existe uma matrícula ativa para este aluno, curso e modalidade.");
        }

        return await GetByIdAsync(enrollment.Id);
    }

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> ChangeStatusAsync(int enrollmentId, EnrollmentStatusChangeDto dto)
    {
        var enrollment = await _context.Enrollments
            .Include(current => current.Status)
            .SingleOrDefaultAsync(current => current.Id == enrollmentId);
        if (enrollment is null)
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Matrícula não encontrada.");

        var targetStatus = await _context.EnrollmentStatuses.SingleOrDefaultAsync(status => status.Id == dto.StatusId);
        if (targetStatus is null)
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Status de matrícula não encontrado.");
        if (!IsTransitionAllowed(enrollment.Status.Code, targetStatus.Code))
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Transição de status não permitida.");

        if (targetStatus.Code == Approved && !enrollment.IsActiveEnrollment &&
            await _context.Enrollments.AnyAsync(current =>
                current.Id != enrollment.Id &&
                current.StudentId == enrollment.StudentId &&
                current.CourseId == enrollment.CourseId &&
                current.FormatId == enrollment.FormatId &&
                current.IsActiveEnrollment))
        {
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Já existe uma matrícula ativa para este aluno, curso e modalidade.");
        }

        enrollment.StatusId = targetStatus.Id;
        enrollment.Status = targetStatus;
        enrollment.IsActiveEnrollment = targetStatus.Code == Approved;
        enrollment.CompletionDate = targetStatus.Code == Completed ? DateTime.UtcNow : null;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiResponseDto<EnrollmentResponseDTO>.Error("Já existe uma matrícula ativa para este aluno, curso e modalidade.");
        }

        return await GetByIdAsync(enrollment.Id);
    }

    public async Task<ApiResponseDto<EnrollmentResponseDTO>> GetByIdAsync(int enrollmentId)
    {
        var enrollment = await Project().FirstOrDefaultAsync(current => current.Id == enrollmentId);
        return enrollment is null
            ? ApiResponseDto<EnrollmentResponseDTO>.Error("Matrícula não encontrada.")
            : new ApiResponseDto<EnrollmentResponseDTO> { Data = enrollment };
    }

    public async Task<ApiResponseDto<List<EnrollmentResponseDTO>>> GetByStudentAsync(int studentId) =>
        new() { Data = await Project().Where(current => current.StudentId == studentId).ToListAsync() };

    private IQueryable<EnrollmentResponseDTO> Project() => _context.Enrollments.AsNoTracking().Select(enrollment => new EnrollmentResponseDTO
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

    private static bool IsTransitionAllowed(string current, string target) => (current, target) switch
    {
        (Pending, Approved or Cancelled) => true,
        (Approved, Suspended or Completed or Cancelled) => true,
        (Suspended, Approved or Cancelled) => true,
        _ => false
    };
}
