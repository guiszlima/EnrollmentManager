using EnrollmentManager.API.Configurations;
using EnrollmentManager.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnrollmentManager.API.Data.Seeders;

public static class AdminSeeder
{
    public static async Task SeedAsync(
    ApplicationDbContext context,
    IOptions<AdminConfiguration> adminConfiguration,
    IPasswordHasher<User> passwordHasher)
    {
        var adminEmail = adminConfiguration.Value.Email;
        var adminPassword = adminConfiguration.Value.Password;

        if (string.IsNullOrWhiteSpace(adminEmail))
            throw new InvalidOperationException("Admin email is not configured.");

        if (string.IsNullOrWhiteSpace(adminPassword))
            throw new InvalidOperationException("Admin password is not configured.");

        var adminRole = await context.Roles
            .SingleOrDefaultAsync(x => x.Name == "Admin");

        if (adminRole is null)
            throw new InvalidOperationException(
                "Admin role was not found. Run the reference data seed first.");

        var existingAdmin = await context.Users
            .SingleOrDefaultAsync(x => x.UserName == "Administrator");

        if (existingAdmin is not null)
        {
            existingAdmin.Email = adminEmail;
            existingAdmin.RoleId = adminRole.Id;
            existingAdmin.IsActive = true;

            await context.SaveChangesAsync();

            return;
        }

        var admin = new User
        {
            UserName = "Administrator",
            Email = adminEmail,
            RoleId = adminRole.Id,
            IsActive = true
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            adminPassword);

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}
