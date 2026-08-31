using EnrollmentManager.API.Data;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Catalogs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.Tests.Services;

public class EducationLevelServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Education_Levels()
    {
        await using var context = CreateContext();
        context.EducationLevels.Add(new EducationLevel { Id = 1, Name = "Graduação" });
        await context.SaveChangesAsync();

        var result = await new EducationLevelService(context).GetAllAsync();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new { Id = 1, Name = "Graduação" });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Education_Level()
    {
        await using var context = CreateContext();
        context.EducationLevels.Add(new EducationLevel { Id = 1, Name = "Pós-graduação" });
        await context.SaveChangesAsync();

        var result = await new EducationLevelService(context).GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new { Id = 1, Name = "Pós-graduação" });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Education_Level_Does_Not_Exist()
    {
        await using var context = CreateContext();

        var result = await new EducationLevelService(context).GetByIdAsync(99);

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