using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Interfaces.Course;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Courses;

public class CourseStatusService : ICourseStatusService
{
    private readonly ApplicationDbContext _context;

    public CourseStatusService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseStatusResponseDto>> GetAllAsync()
    {
        return await _context.CourseStatuses
            .AsNoTracking()
            .Select(cs => new CourseStatusResponseDto
            {
                Id = cs.Id,
                Name = cs.Name,
                Code = cs.Code,
                Description = cs.Description
            })
            .ToListAsync();
    }

    public async Task<CourseStatusResponseDto?> GetByIdAsync(int id)
    {
        return await _context.CourseStatuses
            .AsNoTracking()
            .Where(cs => cs.Id == id)
            .Select(cs => new CourseStatusResponseDto
            {
                Id = cs.Id,
                Name = cs.Name,
                Code = cs.Code,
                Description = cs.Description
            })
            .FirstOrDefaultAsync();
    }

}