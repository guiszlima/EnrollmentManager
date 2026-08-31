using EnrollmentManager.API.Data;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Catalogs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.Tests.Services;

public class EnrollmentStatusServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Enrollment_Statuses()
    {
        await using var context = CreateContext();
        context.EnrollmentStatuses.Add(new EnrollmentStatus
        {
            Id = 1,
            Name = "Pendente",
            Code = "PENDING",
            Description = "Aguardando análise"
        });
        await context.SaveChangesAsync();

        var result = await new EnrollmentStatusService(context).GetAllAsync();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                Id = 1,
                Name = "Pendente",
                Code = "PENDING",
                Description = "Aguardando análise"
            });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Enrollment_Status()
    {
        await using var context = CreateContext();
        context.EnrollmentStatuses.Add(new EnrollmentStatus
        {
            Id = 1,
            Name = "Ativo",
            Code = "ACTIVE"
        });
        await context.SaveChangesAsync();

        var result = await new EnrollmentStatusService(context).GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new
        {
            Id = 1,
            Name = "Ativo",
            Code = "ACTIVE",
            Description = (string?)null
        });
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Enrollment_Status_Does_Not_Exist()
    {
        await using var context = CreateContext();

        var result = await new EnrollmentStatusService(context).GetByIdAsync(99);

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