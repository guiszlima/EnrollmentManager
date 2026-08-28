using EnrollmentManager.API.Data;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Courses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.Tests.Services;

public class CourseStatusServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Course_Statuses()
    {
        await using var context = CreateContext();
        context.CourseStatuses.Add(new CourseStatus
        {
            Id = 1,
            Name = "Ativo",
            Code = "ACTIVE",
            Description = "Curso disponível"
        });
        await context.SaveChangesAsync();

        var result = await new CourseStatusService(context).GetAllAsync();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                Id = 1,
                Name = "Ativo",
                Code = "ACTIVE",
                Description = "Curso disponível"
            });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Course_Status()
    {
        await using var context = CreateContext();
        context.CourseStatuses.Add(new CourseStatus
        {
            Id = 1,
            Name = "Descontinuado",
            Code = "DISCONTINUED"
        });
        await context.SaveChangesAsync();

        var result = await new CourseStatusService(context).GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new
        {
            Id = 1,
            Name = "Descontinuado",
            Code = "DISCONTINUED",
            Description = (string?)null
        });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Course_Status_Does_Not_Exist()
    {
        await using var context = CreateContext();

        var result = await new CourseStatusService(context).GetByIdAsync(99);

        result.Should().BeNull();
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}