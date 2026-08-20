using EnrollmentManager.API.Configurations;

namespace EnrollmentManager.API.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddApplicationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailConfiguration>(
            configuration.GetSection("Email")
        );
        services.Configure<AppConfiguration>(
            configuration.GetSection("Frontendurl")
            
            );
        return services;
    }
}