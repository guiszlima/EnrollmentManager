using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Catalogs;

public class CourseStudyFormatService : ICourseStudyFormatService
{
    private readonly ApplicationDbContext _context;

    public CourseStudyFormatService(ApplicationDbContext context) => _context = context;

    public async Task<List<CourseStudyFormatResponseDto>> GetAllAsync() =>
        await _context.CourseStudyFormats.AsNoTracking().Select(item => new CourseStudyFormatResponseDto
        {
            CourseId = item.CourseId,
            CourseName = item.Course.Name,
            FormatId = item.FormatId,
            FormatName = item.Format.Name
        }).ToListAsync();

    public async Task<CourseStudyFormatResponseDto?> GetByIdAsync(int courseId, int formatId) =>
        await _context.CourseStudyFormats.AsNoTracking()
            .Where(item => item.CourseId == courseId && item.FormatId == formatId)
            .Select(item => new CourseStudyFormatResponseDto
            {
                CourseId = item.CourseId,
                CourseName = item.Course.Name,
                FormatId = item.FormatId,
                FormatName = item.Format.Name
            }).FirstOrDefaultAsync();
}