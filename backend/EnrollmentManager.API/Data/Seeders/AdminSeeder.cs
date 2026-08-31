using EnrollmentManager.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Data.Seeders;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IConfiguration configuration,
        IPasswordHasher<User> passwordHasher)
    {
        var adminEmail = configuration["ADMIN:EMAIL"];
        var adminPassword = configuration["ADMIN:PASSWORD"];

        if (string.IsNullOrWhiteSpace(adminEmail))
            throw new InvalidOperationException(
                "Admin email is not configured.");

        if (string.IsNullOrWhiteSpace(adminPassword))
            throw new InvalidOperationException(
                "Admin password is not configured.");

        var adminRole = await context.Roles
            .SingleOrDefaultAsync(x => x.Name == "Admin");

        if (adminRole is null)
            throw new InvalidOperationException(
                "Admin role was not found. Run the reference data seed first.");

        var existingAdmin = await context.Users
            .SingleOrDefaultAsync(x => x.Email == adminEmail);

        if (existingAdmin is not null)
            throw new InvalidOperationException(
                "Admin already exists.");

        var admin = new User
        {
            UserName = "Administrator",
            Email = adminEmail,
            RoleId = adminRole.Id,
            IsActive = true
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            adminPassword
        );

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}