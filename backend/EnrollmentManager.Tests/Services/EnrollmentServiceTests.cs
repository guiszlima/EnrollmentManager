using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Enrollment;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Enrollments;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace EnrollmentManager.Tests.Services;

public class EnrollmentServiceTests
{
    // Helper privado para instanciar o serviço com NullLogger
    private static EnrollmentService CreateService(ApplicationDbContext context)
    {
        return new EnrollmentService(context, NullLogger<EnrollmentService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Pending_Enrollment_Without_Client_Status()
    {
        await using var context = CreateContext();
        await SeedAsync(context);

        var service = CreateService(context);
        var result = await service.CreateAsync(new EnrollmentCreateDTO
        {
            StudentId = 1, CourseId = 1, FormatId = 1
        });

        result.Errors.Should().BeEmpty();
        result.Data!.StatusName.Should().Be("Pendente");
        result.Data.CompletionDate.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Duplicate_Active_Or_Pending_Enrollment()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var service = CreateService(context);

        // Primeira matrícula cria pendente com sucesso
        await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        // Segunda tentativa deve falhar
        var result = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        result.Errors.Should().ContainSingle()
            .Which.Should().Be("Você já possui uma matrícula ativa ou pendente para este curso e modalidade.");
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Discontinued_Course()
    {
        await using var context = CreateContext();
        await SeedAsync(context, courseCode: "DISCONTINUED");

        var service = CreateService(context);
        var result = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        result.Errors.Should().ContainSingle().Which.Should().Be("Curso não está disponível para novas matrículas.");
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Inactive_Student_User()
    {
        await using var context = CreateContext();
        await SeedAsync(context, userActive: false);

        var service = CreateService(context);
        var result = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        result.Errors.Should().ContainSingle().Which.Should().Be("Usuário do aluno está inativo.");
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Format_Not_Associated_With_Course()
    {
        await using var context = CreateContext();
        await SeedAsync(context, addCourseFormat: false);

        var service = CreateService(context);
        var result = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        result.Errors.Should().ContainSingle().Which.Should().Be("A modalidade informada não é aceita por este curso ou não existe.");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Enrollment_When_Exists()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var service = CreateService(context);
        var created = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        var result = await service.GetByIdAsync(created.Data!.Id);

        result.Errors.Should().BeEmpty();
        result.Data.Should().NotBeNull();
        result.Data!.StudentId.Should().Be(1);
    }

    [Fact]
    public async Task GetByStudentAsync_Should_Return_List_Of_Enrollments()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var service = CreateService(context);
        await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        var result = await service.GetByStudentAsync(1);

        result.Errors.Should().BeEmpty();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task ChangeStatusAsync_Should_Follow_Workflow_And_Set_Completion_Date()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var service = CreateService(context);
        var created = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        var approved = await service.ChangeStatusAsync(created.Data!.Id, new EnrollmentStatusChangeDto { StatusId = 2 });
        var completed = await service.ChangeStatusAsync(created.Data.Id, new EnrollmentStatusChangeDto { StatusId = 5 });

        approved.Errors.Should().BeEmpty();
        approved.Data!.CompletionDate.Should().BeNull();
        approved.Data.StatusName.Should().Be("Ativa");
        completed.Errors.Should().BeEmpty();
        completed.Data!.CompletionDate.Should().BeOnOrAfter(completed.Data.EnrollmentDate);
    }

    [Fact]
    public async Task ChangeStatusAsync_Should_Reject_Invalid_Transition()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var service = CreateService(context);
        var created = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        var result = await service.ChangeStatusAsync(created.Data!.Id, new EnrollmentStatusChangeDto { StatusId = 5 });

        result.Errors.Should().ContainSingle().Which.Should().Be("Transição de status não permitida.");
    }

    [Fact]
    public async Task ChangeStatusAsync_Should_Allow_New_Active_Enrollment_After_Cancellation()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var service = CreateService(context);
        var first = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });
        await service.ChangeStatusAsync(first.Data!.Id, new EnrollmentStatusChangeDto { StatusId = 4 });
        var second = await service.CreateAsync(new EnrollmentCreateDTO { StudentId = 1, CourseId = 1, FormatId = 1 });

        var result = await service.ChangeStatusAsync(second.Data!.Id, new EnrollmentStatusChangeDto { StatusId = 2 });

        result.Errors.Should().BeEmpty();
    }

    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task SeedAsync(ApplicationDbContext context, bool userActive = true, string courseCode = "ACTIVE", bool addCourseFormat = true)
    {
        var role = new Role { Id = 1, Name = "Student" };
        var user = new User { Id = 1, UserName = "Student", Email = "student@test.com", PasswordHash = "hash", IsActive = userActive, Role = role };
        var student = new Student { UserId = 1, User = user, Cpf = "12345678901", RegistrationNumber = "REG-1", BirthDate = new DateTime(2000, 1, 1), Phone = "12345678", Address = "Test Address" };
        var courseStatus = new CourseStatus { Id = 1, Name = courseCode == "ACTIVE" ? "Active" : "Discontinued", Code = courseCode };
        var course = new Course { Id = 1, Name = "Course", CourseType = new CourseType { Id = 1, Name = "Type" }, EducationLevel = new EducationLevel { Id = 1, Name = "Level" }, CourseStatus = courseStatus };
        var format = new StudyFormat { Id = 1, Name = "EAD" };
        context.AddRange(role, user, student, course, format);
        if (addCourseFormat)
            context.CourseStudyFormats.Add(new CourseStudyFormat { Course = course, Format = format, CourseId = 1, FormatId = 1 });
        context.EnrollmentStatuses.AddRange(
            new EnrollmentStatus { Id = 1, Name = "Pendente", Code = "PENDING" },
            new EnrollmentStatus { Id = 2, Name = "Ativa", Code = "APPROVED" },
            new EnrollmentStatus { Id = 3, Name = "Trancada", Code = "SUSPENDED" },
            new EnrollmentStatus { Id = 4, Name = "Cancelada", Code = "CANCELLED" },
            new EnrollmentStatus { Id = 5, Name = "Concluída", Code = "COMPLETED" });
        await context.SaveChangesAsync();
    }
}