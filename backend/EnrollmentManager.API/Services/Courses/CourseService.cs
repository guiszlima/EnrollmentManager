using System.Linq.Expressions;
using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Course;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Interfaces.Course;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Courses;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context) => _context = context;

    public async Task<List<CourseResponseDto>> GetAllAsync(CourseFilterDto filter)
    {
        var query = _context.Courses.AsNoTracking().AsQueryable();

        if (filter.CourseTypeId.HasValue)
            query = query.Where(course => course.CourseTypeId == filter.CourseTypeId.Value);
        if (filter.EducationLevelId.HasValue)
            query = query.Where(course => course.EducationLevelId == filter.EducationLevelId.Value);
        if (filter.StatusId.HasValue)
            query = query.Where(course => course.CourseStatusId == filter.StatusId.Value);
        if (filter.FormatIds.Count > 0)
            query = query.Where(course => filter.FormatIds.All(formatId =>
                course.AllowedFormats.Any(format => format.FormatId == formatId)));

        return await query
            .AsNoTracking()
            .Select(ProjectToDto())
            .ToListAsync();
    }

    public async Task<CourseResponseDto?> GetByIdAsync(int id) =>
        await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(ProjectToDto())
            .FirstOrDefaultAsync();

    public async Task<CourseResponseDto?> CreateAsync(CourseInputDto dto)
    {
        if (!await HasValidClassificationsAsync(dto))
            throw new CourseValidationException("Classificações do curso inválidas.");

        await ValidateRelationsAsync(dto);

        var course = new Course
        {
            Name = dto.Name,
            CourseTypeId = dto.CourseTypeId,
            EducationLevelId = dto.EducationLevelId,
            CourseStatusId = dto.CourseStatusId,
            TotalSlots = dto.TotalSlots,
            AvailableSlots = dto.TotalSlots
        };

        AddRelations(course, dto);

        _context.Courses.Add(course);

        try
        {
            await _context.SaveChangesAsync();
            return await GetByIdAsync(course.Id);
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public async Task<CourseResponseDto?> UpdateAsync(int id, CourseInputDto dto)
    {
        var course = await _context.Courses
            .Include(current => current.AllowedFormats)
            .Include(current => current.CourseTeachers)
            .FirstOrDefaultAsync(current => current.Id == id);
        if (course is null)
            return null;

        if (!await HasValidClassificationsAsync(dto))
            throw new CourseValidationException("Curso não encontrado ou classificações inválidas.");

        await ValidateRelationsAsync(dto);

        course.Name = dto.Name;
        course.CourseTypeId = dto.CourseTypeId;
        course.EducationLevelId = dto.EducationLevelId;
        course.CourseStatusId = dto.CourseStatusId;
        var usedSlots = course.TotalSlots - course.AvailableSlots;
        if (dto.TotalSlots < usedSlots)
            throw new CourseValidationException("O limite de vagas não pode ser menor que as matrículas existentes.");

        course.TotalSlots = dto.TotalSlots;
        course.AvailableSlots = dto.TotalSlots - usedSlots;
        course.AllowedFormats.Clear();
        course.CourseTeachers.Clear();
        AddRelations(course, dto);

        try
        {
            await _context.SaveChangesAsync();
            return await GetByIdAsync(course.Id);
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _context.Enrollments.AnyAsync(e => e.CourseId == id))
            return false;

        var rowsAffected = await _context.Courses
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

        return rowsAffected > 0;
    }

    // Projeção encapsulada e reutilizável
    private static Expression<Func<Course, CourseResponseDto>> ProjectToDto() => c => new CourseResponseDto
    {
        Id = c.Id,
        Name = c.Name,
        CourseTypeId = c.CourseTypeId,
        CourseTypeName = c.CourseType.Name,
        EducationLevelId = c.EducationLevelId,
        EducationLevelName = c.EducationLevel.Name,
        StatusId = c.CourseStatusId,
        StatusName = c.CourseStatus.Name,
        FormatIds = c.AllowedFormats.Select(format => format.FormatId).ToList(),
        TeacherIds = c.CourseTeachers.Select(teacher => teacher.TeacherId).ToList(),
        TotalSlots = c.TotalSlots,
        AvailableSlots = c.AvailableSlots,
        EnrollmentCount = c.Enrollments.Count(enrollment => enrollment.ConsumesSeat),
        Enrollments = c.Enrollments.Select(enrollment => new CourseEnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            StudentName = enrollment.Student.User.UserName,
            StudentEmail = enrollment.Student.User.Email,
            RegistrationNumber = enrollment.Student.RegistrationNumber ?? string.Empty,
            FormatId = enrollment.FormatId,
            FormatName = enrollment.Format.Name,
            StatusId = enrollment.StatusId,
            StatusName = enrollment.Status.Name,
            EnrollmentDate = enrollment.EnrollmentDate
        }).ToList()
    };

    // Validação otimizada em uma única consulta SQL
    private async Task<bool> HasValidClassificationsAsync(CourseInputDto dto)
    {
        var validCount = await _context.CourseTypes.Where(t => t.Id == dto.CourseTypeId).Select(_ => 1)
            .Concat(_context.EducationLevels.Where(e => e.Id == dto.EducationLevelId).Select(_ => 1))
            .Concat(_context.CourseStatuses.Where(s => s.Id == dto.CourseStatusId).Select(_ => 1))
            .CountAsync();

        return validCount == 3;
    }

    private async Task ValidateRelationsAsync(CourseInputDto dto)
    {
        var formatIds = dto.FormatIds.Distinct().ToList();
        if (formatIds.Count == 0)
            throw new CourseValidationException("É necessário informar pelo menos um formato de estudo.");

        var existingFormatCount = await _context.StudyFormats
            .CountAsync(format => formatIds.Contains(format.Id));

        if (existingFormatCount != formatIds.Count)
            throw new CourseValidationException("Um ou mais formatos do curso não existem.");

        var teacherIds = dto.TeacherIds.Distinct().ToList();
        if (teacherIds.Count == 0)
            return;

        var teachers = await _context.Teachers
            .Where(teacher => teacherIds.Contains(teacher.UserId))
            .Select(teacher => new
            {
                teacher.UserId,
                FormatIds = teacher.AllowedFormats.Select(format => format.FormatId).ToList()
            })
            .ToListAsync();

        if (teachers.Count != teacherIds.Count ||
            teachers.Any(teacher => formatIds.Any(formatId => !teacher.FormatIds.Contains(formatId))))
        {
            throw new CourseValidationException("Professor não tem disponiblidade para lecionar curso");
        }
    }

    private static void AddRelations(Course course, CourseInputDto dto)
    {
        foreach (var formatId in dto.FormatIds.Distinct())
            course.AllowedFormats.Add(new CourseStudyFormat { Course = course, FormatId = formatId });

        foreach (var teacherId in dto.TeacherIds.Distinct())
            course.CourseTeachers.Add(new CourseTeacher { Course = course, TeacherId = teacherId });
    }
}
