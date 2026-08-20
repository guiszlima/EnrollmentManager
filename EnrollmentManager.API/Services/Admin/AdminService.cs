using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Dtos.User;
using EnrollmentManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Admin;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponseDto<List<AdminUserDto>>> GetUsersAsync(bool active)
    {
        var users = await _context.Users
            .Where(u => u.IsActive == active)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email,
                Role = u.Role!.Name,
                Status = u.IsActive
            })
            .ToListAsync();

        return new ApiResponseDto<List<AdminUserDto>>(Data: users);
    }

    public async Task<ApiResponseDto<AdminUserDto>> ChangeUserRoleAsync(
        int userId,
        ChangeUserRoleDto dto)
    {
        User? user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ApiResponseDto<AdminUserDto>(Errors: ["Usuário não encontrado."]);

        Role? role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == dto.RoleId);

        if (role is null)
            return new ApiResponseDto<AdminUserDto>(Errors: ["Cargo não encontrado."]);

        user.RoleId = role.Id;
        user.Role = role;

        await _context.SaveChangesAsync();

        AdminUserDto result = MapToAdminUserDto(user);

        return new ApiResponseDto<AdminUserDto>(Data: result);
    }

    public async Task<ApiResponseDto<bool>> DeleteUserAsync(int userId)
    {
        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ApiResponseDto<bool>(Errors: ["Usuário não encontrado."]);

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return new ApiResponseDto<bool>(
            Data: true,
            Message: "Usuário removido com sucesso."
        );
    }

    public async Task<ApiResponseDto<AdminUserDto>> ApproveUserAsync(int userId)
    {
        User? user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ApiResponseDto<AdminUserDto>(Errors: ["Usuário não encontrado."]);

        if (user.IsActive)
            return new ApiResponseDto<AdminUserDto>(Errors: ["Usuário já está ativo."]);

        user.IsActive = true;

        await _context.SaveChangesAsync();

        AdminUserDto result = MapToAdminUserDto(user);

        return new ApiResponseDto<AdminUserDto>(
            Data: result,
            Message: "Usuário aceito com sucesso."
        );
    }

    private static AdminUserDto MapToAdminUserDto(User user)
    {
        return new AdminUserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Role = user.Role!.Name,
            Status = user.IsActive
        };
    }
}