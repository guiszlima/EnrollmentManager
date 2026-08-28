using EnrollmentManager.API.Data;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Catalogs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.Tests.Services;

public class CourseStudyFormatServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Course_Study_Formats_With_Related_Names()
    {
        await using var context = CreateContext();
        AddCourseStudyFormat(context);
        await context.SaveChangesAsync();

        var result = await new CourseStudyFormatService(context).GetAllAsync();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                CourseId = 1,
                CourseName = "Ciência da Computação",
                FormatId = 2,
                FormatName = "EAD"
            });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Course_Study_Format()
    {
        await using var context = CreateContext();
        AddCourseStudyFormat(context);
        await context.SaveChangesAsync();

        var result = await new CourseStudyFormatService(context).GetByIdAsync(1, 2);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new
        {
            CourseId = 1,
            CourseName = "Ciência da Computação",
            FormatId = 2,
            FormatName = "EAD"
        });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Combination_Does_Not_Exist()
    {
        await using var context = CreateContext();

        var result = await new CourseStudyFormatService(context).GetByIdAsync(1, 2);

        result.Should().BeNull();
    }

    private static void AddCourseStudyFormat(ApplicationDbContext context)
    {
        var course = new Course
        {
            Id = 1,
            Name = "Ciência da Computação",
            CourseType = new CourseType { Id = 10, Name = "Bacharelado" },
            EducationLevel = new EducationLevel { Id = 20, Name = "Graduação" },
            CourseStatus = new CourseStatus { Id = 30, Name = "Ativo", Code = "ACTIVE" }
        };
        var format = new StudyFormat { Id = 2, Name = "EAD" };

        context.CourseStudyFormats.Add(new CourseStudyFormat
        {
            CourseId = course.Id,
            Course = course,
            FormatId = format.Id,
            Format = format
        });
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}