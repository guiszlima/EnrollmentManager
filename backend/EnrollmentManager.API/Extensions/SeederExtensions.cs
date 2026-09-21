using EnrollmentManager.API.Configurations;
using EnrollmentManager.API.Data;
using EnrollmentManager.API.Data.Seeders;
using EnrollmentManager.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EnrollmentManager.API.Extensions;

public static class SeederExtensions
{
    public static async Task AddSeederExtension(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var scopedProvider = scope.ServiceProvider;

        var context = scopedProvider.GetRequiredService<ApplicationDbContext>();

        var adminConfiguration = scopedProvider.GetRequiredService<IOptions<AdminConfiguration>>();

        var passwordHasher = scopedProvider.GetRequiredService<IPasswordHasher<User>>();

        // Aplica migrations pendentes
        await context.Database.MigrateAsync();

        // Usuário administrador (Removido a linha do ReferenceDataSeeder que não existia)
        await AdminSeeder.SeedAsync(
            context,
            adminConfiguration,
            passwordHasher
        );
    }
}
