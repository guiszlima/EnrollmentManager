using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.User;
using EnrollmentManager.API.Models;
using Microsoft.EntityFrameworkCore;
using EnrollmentManager.API.DTOs.Admin;
using EnrollmentManager.API.Constants;

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

        return new ApiResponseDto<List<AdminUserDto>> { Data = users };
    }

    public async Task<ApiResponseDto<AdminUserDto>> ChangeUserRoleAsync(
        int userId,
        ChangeUserRoleDto dto)
    {
        User? user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return ApiResponseDto<AdminUserDto>.Error("Usuário não encontrado.");

        Role? role = await _context.Roles
        .FirstOrDefaultAsync(r => r.Id == dto.RoleId);

        if (role is null)
            return ApiResponseDto<AdminUserDto>.Error("Cargo não encontrado.");

        if (user.RoleId != role.Id && IsStudentOrTeacher(user.Role?.Code))
            await SuspendUserEnrollmentsAsync(user.Id);

        user.RoleId = role.Id;
        user.Role = role;

        await _context.SaveChangesAsync();

        AdminUserDto result = MapToAdminUserDto(user);

        return new ApiResponseDto<AdminUserDto> { Data = result };
    }

    public async Task<ApiResponseDto<bool>> DeleteUserAsync(int userId)
    {
        if (userId == 1)
            return ApiResponseDto<bool>.Error(
                "Não é permitido excluir o usuário administrador principal.");

        User? user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return ApiResponseDto<bool>.Error("Usuário não encontrado.");

        if (IsStudentOrTeacher(user.Role?.Code))
            await SuspendUserEnrollmentsAsync(user.Id);

        var hasStudentEnrollments = await _context.Enrollments
            .AnyAsync(enrollment => enrollment.StudentId == user.Id);

        if (hasStudentEnrollments)
        {
            user.IsActive = false;
            user.RoleId = null;
            user.Role = null;
            await _context.SaveChangesAsync();

            return new ApiResponseDto<bool>
            {
                Data = true,
                Message = "Usuário removido com sucesso."
            };
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return new ApiResponseDto<bool> {
            Data = true,
            Message = "Usuário removido com sucesso."
        };
    }

    public async Task<ApiResponseDto<AdminUserDto>> ApproveUserAsync(
        int userId,
        ApproveUserDto dto)
    {
        User? user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return ApiResponseDto<AdminUserDto>.Error(
                "Usuário não encontrado.");

        if (user.IsActive)
            return ApiResponseDto<AdminUserDto>.Error(
                "Usuário já está ativo.");

        Role? role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == dto.RoleId);

        if (role is null)
            return ApiResponseDto<AdminUserDto>.Error(
                "Cargo informado não encontrado.");

        user.RoleId = role.Id;
        user.Role = role;
        user.IsActive = true;

        await _context.SaveChangesAsync();

        AdminUserDto result = MapToAdminUserDto(user);

        return new ApiResponseDto<AdminUserDto>
        {
            Data = result,
            Message = "Usuário aceito com sucesso."
        };
    
}
    private static AdminUserDto MapToAdminUserDto(User user)
    {
        return new AdminUserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Role = user.Role?.Name,
            Status = user.IsActive
        };
    }

    private async Task SuspendUserEnrollmentsAsync(int userId)
    {
        var suspendedStatus = await _context.EnrollmentStatuses
            .SingleAsync(status => status.Code == EnrollmentStatusCodes.Suspended);

        var enrollments = await _context.Enrollments
            .Include(enrollment => enrollment.Status)
            .Include(enrollment => enrollment.Course)
            .Where(enrollment =>
                enrollment.StudentId == userId ||
                enrollment.Course.CourseTeachers.Any(courseTeacher => courseTeacher.TeacherId == userId))
            .ToListAsync();

        foreach (var enrollment in enrollments)
        {
            if (enrollment.ConsumesSeat)
            {
                enrollment.Course.AvailableSlots = Math.Min(
                    enrollment.Course.TotalSlots,
                    enrollment.Course.AvailableSlots + 1);
            }

            enrollment.ApplyStatusChange(
                suspendedStatus,
                EnrollmentStatusCodes.Approved,
                EnrollmentStatusCodes.Completed);
            enrollment.ConsumesSeat = false;
        }
    }

    private static bool IsStudentOrTeacher(string? roleCode) =>
        roleCode is "STUDENT" or "TEACHER";
}
