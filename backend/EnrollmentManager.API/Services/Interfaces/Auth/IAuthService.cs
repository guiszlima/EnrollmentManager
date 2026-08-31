using EnrollmentManager.API.DTOs;
using EnrollmentManager.API.DTOs.Auth;
using EnrollmentManager.API.DTOs.Common;

namespace EnrollmentManager.API.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<ApiResponseDto<string>> RegisterAsync(RegisterUserDto dto);
    Task<ApiResponseDto<string>> LoginAsync(LoginUserDto dto);

   
}
