using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Auth;
using EnrollmentManager.API.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<
            IPasswordHasher<User>,
            PasswordHasher<User>>();

        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}