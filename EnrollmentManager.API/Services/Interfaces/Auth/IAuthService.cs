using EnrollmentManager.API.DTOS;
using EnrollmentManager.API.DTOS.Auth;
using EnrollmentManager.API.DTOs.Common;

namespace EnrollmentManager.API.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<ApiResponseDto<string>> RegisterAsync(RegisterUserDto dto);
    Task<ApiResponseDto<string>> LoginAsync(LoginUserDto dto);

   
}