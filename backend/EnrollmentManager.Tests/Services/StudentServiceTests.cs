using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Student;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Students;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.Tests.Services;

public class StudentServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Brazilian_Student_Successfully()
    {
        await using var context = CreateContext();


    var studentRole = CreateStudentRole();
        var studyFormat = CreateStudyFormat();

        context.Users.Add(CreateUser(1, studentRole));
        context.StudyFormats.Add(studyFormat);

        await context.SaveChangesAsync();

        var result = await new StudentService(context).CreateAsync(
            CreateDto(
                1,
                nationality: "Brasil",
                cpf: "12345678901",
                passport: null));

        result.Errors.Should().BeEmpty();
        result.Data.Should().NotBeNull();
        result.Data!.Nationality.Should().Be("Brasil");
        result.Data.Cpf.Should().Be("12345678901");
        result.Data.PassportNumber.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_International_Student_Successfully()
    {
        await using var context = CreateContext();

        var studentRole = CreateStudentRole();
        var studyFormat = CreateStudyFormat();

        context.Users.Add(CreateUser(1, studentRole));
        context.StudyFormats.Add(studyFormat);

        await context.SaveChangesAsync();

        var result = await new StudentService(context).CreateAsync(
            CreateDto(
                1,
                nationality: "Argentina",
                cpf: "99999999999",
                passport: "PASSPORT-123"));

        result.Errors.Should().BeEmpty();
        result.Data.Should().NotBeNull();
        result.Data!.Nationality.Should().Be("Argentina");
        result.Data.PassportNumber.Should().Be("PASSPORT-123");
        result.Data.Cpf.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_When_Brazilian_Without_Cpf()
    {
        await using var context = CreateContext();

        var studentRole = CreateStudentRole();
        context.Users.Add(CreateUser(1, studentRole));

        await context.SaveChangesAsync();

        var result = await new StudentService(context).CreateAsync(
            CreateDto(
                1,
                nationality: "Brasil",
                cpf: null,
                passport: "PASS-123"));

        result.Errors.Should().ContainSingle()
            .Which.Should().Be(
                "Documento de identificação inválido para a nacionalidade informada.");
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Duplicate_Cpf()
    {
        await using var context = CreateContext();

        var studentRole = CreateStudentRole();
        var studyFormat = CreateStudyFormat();

        var firstUser = CreateUser(1, studentRole);
        var secondUser = CreateUser(2, studentRole);

        context.AddRange(firstUser, secondUser);
        context.StudyFormats.Add(studyFormat);

        context.Students.Add(new Student
        {
            UserId = 1,
            User = firstUser,
            Cpf = "11111111111",
            RegistrationNumber = "REG-1",
            BirthDate = new DateTime(2000, 1, 1),
            Phone = "12345678",
            Address = "Test Address",
            Nationality = "Brasil"
        });

        await context.SaveChangesAsync();

        var result = await new StudentService(context).CreateAsync(
            CreateDto(
                2,
                nationality: "Brasil",
                cpf: "11111111111",
                passport: null));

        result.Errors.Should().ContainSingle()
            .Which.Should().Be(
                "Já existe um aluno cadastrado com esta Matrícula, CPF ou Passaporte.");
    }

    [Fact]
    public async Task DeleteAsync_Should_Prevent_Deletion_If_Enrollments_Exist()
    {
        await using var context = CreateContext();

        var studentRole = CreateStudentRole();
        var user = CreateUser(1, studentRole);

        var student = new Student
        {
            UserId = 1,
            User = user,
            Cpf = "11111111111",
            RegistrationNumber = "REG-1",
            Nationality = "Brasil"
        };

        context.Users.Add(user);
        context.Students.Add(student);

        context.Enrollments.Add(new Enrollment
        {
            Id = 1,
            StudentId = 1,
            CourseId = 1,
            FormatId = 1,
            StatusId = 1
        });

        await context.SaveChangesAsync();

        var result = await new StudentService(context).DeleteAsync(1);

        result.Errors.Should().ContainSingle()
            .Which.Should().Be(
                "Não é possível remover o aluno, pois existem matrículas ativas vinculadas a ele.");

        result.Data.Should().BeFalse();
    }

    private static ApplicationDbContext CreateContext() =>
        new(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

    private static Role CreateStudentRole() =>
        new()
        {
            Id = 1,
            Name = "Student",
            Code = "STUDENT"
        };

    private static StudyFormat CreateStudyFormat() =>
        new()
        {
            Id = 1,
            Name = "EAD"
        };

    private static User CreateUser(int id, Role role) =>
        new()
        {
            Id = id,
            UserName = $"User {id}",
            Email = $"user{id}@test.com",
            PasswordHash = "hash",
            IsActive = true,
            Role = role
        };

    private static StudentCreateDto CreateDto(
        int userId,
        string nationality,
        string? cpf,
        string? passport,
        string regNumber = "REG-NEW") =>
        new()
        {
            UserId = userId,
            Nationality = nationality,
            Cpf = cpf,
            PassportNumber = passport,
            BirthDate = new DateTime(2000, 1, 1),
            Phone = "12345678",
            Address = "Test Address",
            FormatIds = [1]
        };


}
