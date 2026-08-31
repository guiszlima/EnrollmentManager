using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Catalogs;

public class EnrollmentStatusService : IEnrollmentStatusService
{
    private readonly ApplicationDbContext _context;

    public EnrollmentStatusService(ApplicationDbContext context) => _context = context;

    public async Task<List<EnrollmentStatusResponseDto>> GetAllAsync() =>
        await _context.EnrollmentStatuses.AsNoTracking().Select(status => new EnrollmentStatusResponseDto
        {
            Id = status.Id,
            Name = status.Name,
            Code = status.Code,
            Description = status.Description
        }).ToListAsync();

    public async Task<EnrollmentStatusResponseDto?> GetByIdAsync(int id) =>
        await _context.EnrollmentStatuses.AsNoTracking().Where(status => status.Id == id)
            .Select(status => new EnrollmentStatusResponseDto
            {
                Id = status.Id,
                Name = status.Name,
                Code = status.Code,
                Description = status.Description
            }).FirstOrDefaultAsync();
}