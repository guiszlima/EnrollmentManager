using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOS.Student;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Students;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.Tests.Services;

public class StudentServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Require_Cpf_Or_Passport()
    {
        await using var context = CreateContext();
        context.Users.Add(CreateUser(1));
        await context.SaveChangesAsync();

        var result = await new StudentService(context).CreateAsync(CreateDto(1));

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Duplicate_Registration_Number()
    {
        await using var context = CreateContext();
        var firstUser = CreateUser(1);
        var secondUser = CreateUser(2);
        context.AddRange(firstUser, secondUser);
        context.Students.Add(new Student
        {
            UserId = 1, User = firstUser, Cpf = "12345678901", RegistrationNumber = "REG-1",
            BirthDate = new DateTime(2000, 1, 1), Phone = "12345678", Address = "Test Address"
        });
        await context.SaveChangesAsync();

        var result = await new StudentService(context).CreateAsync(CreateDto(2, passport: "P-2"));

        result.Should().BeNull();
    }

    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static User CreateUser(int id) => new()
    {
        Id = id, UserName = $"User {id}", Email = $"user{id}@test.com", PasswordHash = "hash", IsActive = true
    };

    private static StudentCreateDTO CreateDto(int userId, string? passport = null) => new()
    {
        UserId = userId,
        PassportNumber = passport,
        RegistrationNumber = "REG-1",
        BirthDate = new DateTime(2000, 1, 1),
        Phone = "12345678",
        Address = "Test Address"
    };
}
