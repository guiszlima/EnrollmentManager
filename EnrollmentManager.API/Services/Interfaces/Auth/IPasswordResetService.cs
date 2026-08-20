using EnrollmentManager.API.DTOS.Auth;
using EnrollmentManager.API.DTOs.Common;

namespace EnrollmentManager.API.Services.Interfaces.Auth;


public interface IPasswordResetService
{
    Task<ApiResponseDto<bool>> RequestPasswordResetAsync(int userId);

    Task<ApiResponseDto<bool>> ResetPasswordAsync(
        ResetPasswordDto dto);
}