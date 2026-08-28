using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Admin;
using EnrollmentManager.API.Services.Auth;
using EnrollmentManager.API.Services.Email;
using EnrollmentManager.API.Services.Interfaces;
using EnrollmentManager.API.Services.Interfaces.Auth;
using EnrollmentManager.API.Services.Interfaces.Student;
using EnrollmentManager.API.Services.Interfaces.Role;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using EnrollmentManager.API.Services.Catalogs;
using EnrollmentManager.API.Services.Roles;
using EnrollmentManager.API.Services.Students;
using EnrollmentManager.API.Services.Courses;
using EnrollmentManager.API.Services.Interfaces.Course;
using EnrollmentManager.API.Services.Interfaces.Enrollment;
using EnrollmentManager.API.Services.Enrollments;
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

        services.AddScoped<IPasswordResetService, PasswordResetService>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ICourseTypeService, CourseTypeService>();
        services.AddScoped<IEducationLevelService, EducationLevelService>();
        services.AddScoped<IEnrollmentStatusService, EnrollmentStatusService>();
        services.AddScoped<ICourseStudyFormatService, CourseStudyFormatService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICourseStatusService, CourseStatusService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        return services;
    }
}
