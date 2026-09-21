using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Teacher;
using EnrollmentManager.API.Services.Interfaces.Teacher;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Teachers;

public class TeacherService : ITeacherService
{
    private readonly ApplicationDbContext _context;

    public TeacherService(ApplicationDbContext context) => _context = context;

    public async Task<List<TeacherResponseDto>> GetAllAsync(TeacherFilterDto filter)
    {
        var query = _context.Users
            .AsNoTracking()
            .Where(user => user.Role != null && user.Role.Code == "TEACHER");

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(user => user.UserName.Contains(filter.Name));
        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(user => user.Email.Contains(filter.Email));
        if (filter.StudyFormatId.HasValue)
            query = query.Where(user => user.Teacher != null &&
                user.Teacher.AllowedFormats.Any(format => format.FormatId == filter.StudyFormatId.Value));

        return await query.Select(MapUserToDto()).ToListAsync();
    }

    public async Task<TeacherResponseDto?> GetByIdAsync(int userId) =>
        await _context.Users
            .AsNoTracking()
            .Where(user => user.Id == userId && user.Role != null && user.Role.Code == "TEACHER")
            .Select(MapUserToDto())
            .FirstOrDefaultAsync();

    public async Task<ApiResponseDto<TeacherResponseDto>> CreateAsync(
        int userId,
        IEnumerable<int> formatIds)
    {
        var ids = formatIds.Distinct().ToList();

        if (ids.Count == 0)
            return ApiResponseDto<TeacherResponseDto>.Error("Selecione ao menos um formato de estudo.");

        var user = await _context.Users
            .Include(current => current.Role)
            .FirstOrDefaultAsync(current => current.Id == userId);

        if (user is null)
            return ApiResponseDto<TeacherResponseDto>.Error("Usuário não encontrado.");

        if (user.Role?.Code != "TEACHER")
            return ApiResponseDto<TeacherResponseDto>.Error("O usuário não possui o cargo de professor.");

        if (await _context.Teachers.AnyAsync(teacher => teacher.UserId == userId))
            return ApiResponseDto<TeacherResponseDto>.Error("Este usuário já possui um perfil de professor cadastrado.");

        var validFormatIds = await _context.StudyFormats
            .Where(format => ids.Contains(format.Id))
            .Select(format => format.Id)
            .ToListAsync();

        if (validFormatIds.Count != ids.Count)
            return ApiResponseDto<TeacherResponseDto>.Error("Um ou mais formatos informados não existem.");

        var teacher = new EnrollmentManager.API.Models.Teacher { UserId = userId };
        foreach (var formatId in ids)
            teacher.AllowedFormats.Add(new EnrollmentManager.API.Models.TeacherStudyFormat
            {
                TeacherId = userId,
                FormatId = formatId
            });

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        return new ApiResponseDto<TeacherResponseDto>
        {
            Data = await GetByIdAsync(userId),
            Message = "Professor cadastrado com sucesso."
        };
    }

    public async Task<ApiResponseDto<TeacherResponseDto>> UpdateAsync(
        int userId,
        IEnumerable<int> formatIds)
    {
        var ids = formatIds.Distinct().ToList();
        if (ids.Count == 0)
            return ApiResponseDto<TeacherResponseDto>.Error("Selecione ao menos um formato de estudo.");

        var user = await _context.Users
            .Include(current => current.Role)
            .Include(current => current.Teacher)
                .ThenInclude(current => current!.AllowedFormats)
            .FirstOrDefaultAsync(current => current.Id == userId);

        if (user is null || user.Role?.Code != "TEACHER")
            return ApiResponseDto<TeacherResponseDto>.Error("Professor não encontrado.");

        var validFormatCount = await _context.StudyFormats
            .CountAsync(format => ids.Contains(format.Id));
        if (validFormatCount != ids.Count)
            return ApiResponseDto<TeacherResponseDto>.Error("Um ou mais formatos informados não existem.");

        var teacher = user.Teacher;
        if (teacher is null)
        {
            teacher = new EnrollmentManager.API.Models.Teacher { UserId = userId };
            _context.Teachers.Add(teacher);
        }

        teacher.AllowedFormats.Clear();
        foreach (var formatId in ids)
            teacher.AllowedFormats.Add(new EnrollmentManager.API.Models.TeacherStudyFormat
            {
                TeacherId = userId,
                FormatId = formatId
            });

        await _context.SaveChangesAsync();
        return new ApiResponseDto<TeacherResponseDto>
        {
            Data = await GetByIdAsync(userId),
            Message = "Professor atualizado com sucesso."
        };
    }

    private static System.Linq.Expressions.Expression<Func<EnrollmentManager.API.Models.User, TeacherResponseDto>> MapUserToDto() =>
        user => new TeacherResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FormatIds = user.Teacher == null
                ? new List<int>()
                : user.Teacher.AllowedFormats.Select(format => format.FormatId).ToList()
        };
}
