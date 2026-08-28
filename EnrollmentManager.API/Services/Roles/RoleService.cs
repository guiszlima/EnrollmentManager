using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOS.Role;
using EnrollmentManager.API.Services.Interfaces.Role;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Roles;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleResponseDto>> GetAllAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .Select(role => new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name
            })
            .ToListAsync();
    }

    public async Task<RoleResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(role => role.Id == id)
            .Select(role => new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name
            })
            .FirstOrDefaultAsync();
    }
}