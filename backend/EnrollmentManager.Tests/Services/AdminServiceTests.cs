using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.User;
using EnrollmentManager.API.DTOs.Admin;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Admin;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.Tests.Services;

public class AdminServiceTests
{
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetUsersAsync_Should_Return_Users_With_Requested_Status()
    {
        await using var context = GetInMemoryDbContext();
        var role = new Role { Id = 1, Name = "Student" };
        context.Roles.Add(role);
        context.Users.AddRange(
            new User { UserName = "Active", Email = "active@test.com", PasswordHash = "hash", Role = role, IsActive = true },
            new User { UserName = "Inactive", Email = "inactive@test.com", PasswordHash = "hash", Role = role, IsActive = false });
        await context.SaveChangesAsync();

        var result = await new AdminService(context).GetUsersAsync(true);

        result.Errors.Should().BeEmpty();
        result.Data.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new AdminUserDto
            {
                Id = 1,
                Username = "Active",
                Email = "active@test.com",
                Role = "Student",
                Status = true
            });
    }

    [Fact]
    public async Task ChangeUserRoleAsync_Should_Return_Error_When_User_Does_Not_Exist()
    {
        await using var context = GetInMemoryDbContext();

        var result = await new AdminService(context)
            .ChangeUserRoleAsync(99, new ChangeUserRoleDto { RoleId = 1 });

        result.Errors.Should().ContainSingle().Which.Should().Be("Usuário não encontrado.");
    }

    [Fact]
    public async Task ChangeUserRoleAsync_Should_Return_Error_When_Role_Does_Not_Exist()
    {
        await using var context = GetInMemoryDbContext();
        var currentRole = new Role { Id = 1, Name = "Student" };
        context.Roles.Add(currentRole);
        context.Users.Add(new User
        {
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "hash",
            Role = currentRole
        });
        await context.SaveChangesAsync();
        int userId = await context.Users.Select(user => user.Id).SingleAsync();

        var result = await new AdminService(context)
            .ChangeUserRoleAsync(userId, new ChangeUserRoleDto { RoleId = 2 });

        result.Errors.Should().ContainSingle().Which.Should().Be("Cargo não encontrado.");
    }

    [Fact]
    public async Task ChangeUserRoleAsync_Should_Update_User_Role()
    {
        await using var context = GetInMemoryDbContext();
        var studentRole = new Role { Id = 1, Name = "Student" };
        var adminRole = new Role { Id = 2, Name = "Admin" };
        context.Roles.AddRange(studentRole, adminRole);
        context.Users.Add(new User
        {
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "hash",
            Role = studentRole
        });
        await context.SaveChangesAsync();
        int userId = await context.Users.Select(user => user.Id).SingleAsync();

        var result = await new AdminService(context)
            .ChangeUserRoleAsync(userId, new ChangeUserRoleDto { RoleId = 2 });

        result.Errors.Should().BeEmpty();
        result.Data.Should().BeEquivalentTo(new AdminUserDto
        {
            Id = userId,
            Username = "User",
            Email = "user@test.com",
            Role = "Admin",
            Status = false
        });
        (await context.Users.FindAsync(userId))!.RoleId.Should().Be(2);
    }

    [Fact]
    public async Task DeleteUserAsync_Should_Remove_User()
    {
        await using var context = GetInMemoryDbContext();
        context.Users.Add(new User
        {
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "hash"
        });
        await context.SaveChangesAsync();
        int userId = await context.Users.Select(user => user.Id).SingleAsync();

        var result = await new AdminService(context).DeleteUserAsync(userId);

        result.Data.Should().BeTrue();
        result.Message.Should().Be("Usuário removido com sucesso.");
        (await context.Users.FindAsync(userId)).Should().BeNull();
    }

    [Fact]
    public async Task ApproveUserAsync_Should_Activate_User()
    {
        await using var context = GetInMemoryDbContext();
        var role = new Role { Id = 1, Name = "Student" };
        context.Roles.Add(role);
        context.Users.Add(new User
        {
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "hash",
            Role = role,
            IsActive = false
        });
        await context.SaveChangesAsync();
        int userId = await context.Users.Select(user => user.Id).SingleAsync();

        var result = await new AdminService(context)
            .ApproveUserAsync(userId, new ApproveUserDto { RoleId = 1 });

        result.Errors.Should().BeEmpty();
        result.Data!.Status.Should().BeTrue();
        result.Message.Should().Be("Usuário aceito com sucesso.");
        (await context.Users.FindAsync(userId))!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ApproveUserAsync_Should_Return_Error_When_User_Is_Already_Active()
    {
        await using var context = GetInMemoryDbContext();
        var role = new Role { Id = 1, Name = "Student" };
        context.Roles.Add(role);
        context.Users.Add(new User
        {
            Id = 42,
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "hash",
            Role = role,
            IsActive = true
        });
        await context.SaveChangesAsync();

        var result = await new AdminService(context)
            .ApproveUserAsync(42, new ApproveUserDto { RoleId = 1 });

        result.Errors.Should().ContainSingle().Which.Should().Be("Usuário já está ativo.");
    }
}
