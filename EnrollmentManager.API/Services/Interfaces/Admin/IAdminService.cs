using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.User;
using EnrollmentManager.API.DTOs.Admin;

namespace EnrollmentManager.API.Services.Admin;

public interface IAdminService
{
    Task<ApiResponseDto<List<AdminUserDto>>> GetUsersAsync(bool active);

    Task<ApiResponseDto<AdminUserDto>> ChangeUserRoleAsync(
        int userId,
        ChangeUserRoleDto dto);

    Task<ApiResponseDto<bool>> DeleteUserAsync(int userId);

    Task<ApiResponseDto<AdminUserDto>> ApproveUserAsync(int userId, ApproveUserDto dto);
}
