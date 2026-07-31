using EnrollmentManager.API.DTOS;
using EnrollmentManager.API.DTOS.Auth;

namespace EnrollmentManager.API.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<ApiResponseDTO<string>> RegisterAsync(RegisterUserDTO dto);
    Task<ApiResponseDTO<string>> LoginAsync(LoginUserDTO dto);

   
}