using EnrollmentManager.API.Models;

namespace EnrollmentManager.API.Services.Interfaces.Auth;

public interface ITokenService
{
     string GenerateToken(User user);
}