using EnrollmentManager.API.DTOS.Role;

namespace EnrollmentManager.API.Services.Interfaces.Role;

public interface IRoleService
{
    Task<List<RoleResponseDto>> GetAllAsync();

    Task<RoleResponseDto?> GetByIdAsync(int id);
}