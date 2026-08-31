using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Catalogs;

public class CourseTypeService : ICourseTypeService
{
    private readonly ApplicationDbContext _context;

    public CourseTypeService(ApplicationDbContext context) => _context = context;

    public async Task<List<CourseTypeResponseDto>> GetAllAsync() =>
        await _context.CourseTypes.AsNoTracking().Select(type => new CourseTypeResponseDto
        {
            Id = type.Id,
            Name = type.Name
        }).ToListAsync();

    public async Task<CourseTypeResponseDto?> GetByIdAsync(int id) =>
        await _context.CourseTypes.AsNoTracking().Where(type => type.Id == id)
            .Select(type => new CourseTypeResponseDto
            {
                Id = type.Id,
                Name = type.Name
            }).FirstOrDefaultAsync();
}