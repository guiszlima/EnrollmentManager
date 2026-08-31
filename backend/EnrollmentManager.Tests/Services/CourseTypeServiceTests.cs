using EnrollmentManager.API.Data;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Catalogs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.Tests.Services;

public class CourseTypeServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Course_Types()
    {
        await using var context = CreateContext();
        context.CourseTypes.Add(new CourseType { Id = 1, Name = "Bacharelado" });
        await context.SaveChangesAsync();

        var result = await new CourseTypeService(context).GetAllAsync();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new { Id = 1, Name = "Bacharelado" });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Course_Type()
    {
        await using var context = CreateContext();
        context.CourseTypes.Add(new CourseType { Id = 1, Name = "Tecnólogo" });
        await context.SaveChangesAsync();

        var result = await new CourseTypeService(context).GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new { Id = 1, Name = "Tecnólogo" });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Course_Type_Does_Not_Exist()
    {
        await using var context = CreateContext();

        var result = await new CourseTypeService(context).GetByIdAsync(99);

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