using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Admin;
using EnrollmentManager.API.Services.Auth;
using EnrollmentManager.API.Services.Catalogs;
using EnrollmentManager.API.Services.Courses;
using EnrollmentManager.API.Services.Email;
using EnrollmentManager.API.Services.Enrollments;
using EnrollmentManager.API.Services.Interfaces;
using EnrollmentManager.API.Services.Interfaces.Auth;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using EnrollmentManager.API.Services.Interfaces.Course;
using EnrollmentManager.API.Services.Interfaces.Enrollment;
using EnrollmentManager.API.Services.Interfaces.Role;
using EnrollmentManager.API.Services.Interfaces.Student;
using EnrollmentManager.API.Services.Interfaces.StudyFormat;
using EnrollmentManager.API.Services.Roles;
using EnrollmentManager.API.Services.Students;
using EnrollmentManager.API.Services.StudyFormat;
using EnrollmentManager.API.Services.Interfaces.Teacher;
using EnrollmentManager.API.Services.Teachers;
using Microsoft.AspNetCore.Identity;

namespace EnrollmentManager.API.Extensions;

public static class ServiceCollectionExtensions 
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services
            .AddAuthServices()
            .AddEmailServices()
            .AddAdminServices()
            .AddStudentServices()
            .AddRoleServices()
            .AddCatalogServices()
            .AddCourseServices()
            .AddEnrollmentServices();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<ICourseReportService, CourseReportService>();

        return services;
    }

    private static IServiceCollection AddAuthServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }

    private static IServiceCollection AddEmailServices(
        this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static IServiceCollection AddAdminServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }

    private static IServiceCollection AddStudentServices(
        this IServiceCollection services)
    {
        services.AddScoped<IStudentService, StudentService>();

        return services;
    }

    private static IServiceCollection AddRoleServices(
        this IServiceCollection services)
    {
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }

    private static IServiceCollection AddCatalogServices(
        this IServiceCollection services)
    {
        services.AddScoped<ICourseTypeService, CourseTypeService>();
        services.AddScoped<IEducationLevelService, EducationLevelService>();
        services.AddScoped<IEnrollmentStatusService, EnrollmentManager.API.Services.Catalogs.EnrollmentStatusService>();
        services.AddScoped<ICourseStudyFormatService, CourseStudyFormatService>();
        services.AddScoped<IStudyFormatService, StudyFormatService>();

        return services;
    }

    private static IServiceCollection AddCourseServices(
        this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICourseStatusService, CourseStatusService>();

        return services;
    }

    private static IServiceCollection AddEnrollmentServices(
        this IServiceCollection services)
    {
        services.AddScoped<EnrollmentCreationService>();
        services.AddScoped<EnrollmentManager.API.Services.Enrollments.EnrollmentStatusService>();
        services.AddScoped<EnrollmentQueryService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();

        return services;
    }
}
