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

    public async Task<List<CourseResponseDto>> GetAllAsync() =>
        await _context.Courses
            .AsNoTracking()
            .Select(ProjectToDto())
            .ToListAsync();

    public async Task<CourseResponseDto?> GetByIdAsync(int id) =>
        await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(ProjectToDto())
            .FirstOrDefaultAsync();

    public async Task<CourseResponseDto?> CreateAsync(CourseInputDto dto)
    {
        if (!await HasValidClassificationsAsync(dto))
            return null;

        var course = new Course
        {
            Name = dto.Name,
            CourseTypeId = dto.CourseTypeId,
            EducationLevelId = dto.EducationLevelId,
            CourseStatusId = dto.CourseStatusId
        };

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
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course is null)
            return null;

        if (!await HasValidClassificationsAsync(dto))
            return null;

        course.Name = dto.Name;
        course.CourseTypeId = dto.CourseTypeId;
        course.EducationLevelId = dto.EducationLevelId;
        course.CourseStatusId = dto.CourseStatusId;

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
        StatusName = c.CourseStatus.Name
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
}