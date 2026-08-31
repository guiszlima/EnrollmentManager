using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Catalogs;

public class EducationLevelService : IEducationLevelService
{
    private readonly ApplicationDbContext _context;

    public EducationLevelService(ApplicationDbContext context) => _context = context;

    public async Task<List<EducationLevelResponseDto>> GetAllAsync() =>
        await _context.EducationLevels.AsNoTracking().Select(level => new EducationLevelResponseDto
        {
            Id = level.Id,
            Name = level.Name
        }).ToListAsync();

    public async Task<EducationLevelResponseDto?> GetByIdAsync(int id) =>
        await _context.EducationLevels.AsNoTracking().Where(level => level.Id == id)
            .Select(level => new EducationLevelResponseDto
            {
                Id = level.Id,
                Name = level.Name
            }).FirstOrDefaultAsync();
}