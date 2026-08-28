using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Course;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Interfaces.Course;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Courses;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseResponseDTO>> GetAllAsync()
    {
        IQueryable<Course> query = _context.Courses.AsNoTracking();

        return await query
            .Select(c => new CourseResponseDTO
            {
                Id = c.Id,
                Name = c.Name,

                CourseTypeId = c.CourseTypeId,
                CourseTypeName = c.CourseType.Name,

                EducationLevelId = c.EducationLevelId,
                EducationLevelName = c.EducationLevel.Name,

                StatusId = c.CourseStatusId,
                StatusName = c.CourseStatus.Name
            })
            .ToListAsync();
    }

    public async Task<CourseResponseDTO?> GetByIdAsync(int id)
    {
        IQueryable<Course> query = _context.Courses
            .AsNoTracking();

        return await query
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDTO
            {
                Id = c.Id,
                Name = c.Name,

                CourseTypeId = c.CourseTypeId,
                CourseTypeName = c.CourseType.Name,

                EducationLevelId = c.EducationLevelId,
                EducationLevelName = c.EducationLevel.Name,

                StatusId = c.CourseStatusId,
                StatusName = c.CourseStatus.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CourseResponseDTO?> CreateAsync(CourseInputDTO dto)
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
        }
        catch (DbUpdateException)
        {
            return null;
        }

        return (await GetByIdAsync(course.Id))!;
    }

    public async Task<CourseResponseDTO?> UpdateAsync(
        int id,
        CourseInputDTO dto)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

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
        }
        catch (DbUpdateException)
        {
            return null;
        }

        return await GetByIdAsync(course.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
            return false;

        if (await _context.Enrollments.AnyAsync(enrollment => enrollment.CourseId == id))
            return false;

        _context.Courses.Remove(course);

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<bool> HasValidClassificationsAsync(CourseInputDTO dto) =>
        await _context.CourseTypes.AnyAsync(item => item.Id == dto.CourseTypeId) &&
        await _context.EducationLevels.AnyAsync(item => item.Id == dto.EducationLevelId) &&
        await _context.CourseStatuses.AnyAsync(item => item.Id == dto.CourseStatusId);
}
