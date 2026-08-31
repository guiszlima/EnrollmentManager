using System.Security.Cryptography;
using System.Text.RegularExpressions;
using EnrollmentManager.API.Configurations;
using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Auth;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Auth;
using EnrollmentManager.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace EnrollmentManager.Tests.Services;

public class PasswordResetServiceTests
{
    private readonly Mock<IEmailService> _emailServiceMock = new();

    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private PasswordResetService CreateService(ApplicationDbContext context)
    {
        return new PasswordResetService(
            context,
            _emailServiceMock.Object,
            Options.Create(new AppConfiguration
            {
                FrontendUrl = "https://frontend.test"
            }));
    }

    [Fact]
    public async Task RequestPasswordResetAsync_Should_Return_Error_When_User_Does_Not_Exist()
    {
        await using var context = GetInMemoryDbContext();

        var result = await CreateService(context).RequestPasswordResetAsync(1);

        result.Errors.Should().ContainSingle().Which.Should().Be("Usuário não encontrado.");
        _emailServiceMock.Verify(
            service => service.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_Should_Save_Hashed_Token_And_Send_Reset_Link()
    {
        await using var context = GetInMemoryDbContext();
        context.Users.Add(new User
        {
            Id = 1,
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "old-hash",
            IsActive = true
        });
        await context.SaveChangesAsync();

        string emailBody = string.Empty;
        _emailServiceMock
            .Setup(service => service.SendAsync("user@test.com", It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, _, body) => emailBody = body)
            .Returns(Task.CompletedTask);

        var result = await CreateService(context).RequestPasswordResetAsync(1);

        result.Data.Should().BeTrue();
        result.Message.Should().Be("Instruções para redefinição de senha enviadas ao usuário.");
        emailBody.Should().Contain("https://frontend.test/reset-password?token=");

        System.Text.RegularExpressions.Match tokenMatch =
            Regex.Match(emailBody, "token=([^\\\"&]+)");
        tokenMatch.Success.Should().BeTrue();
        string token = Uri.UnescapeDataString(tokenMatch.Groups[1].Value);
        string expectedHash = Convert.ToHexString(
            SHA256.HashData(Convert.FromBase64String(token)));

        var savedToken = await context.PasswordResetTokens.SingleAsync();
        savedToken.TokenHash.Should().Be(expectedHash);
        savedToken.TokenHash.Should().NotContain(token);
        savedToken.UserId.Should().Be(1);
        savedToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        savedToken.UsedAt.Should().BeNull();
        _emailServiceMock.Verify(
            service => service.SendAsync(
                "user@test.com",
                "Redefinição de senha - Enrollment Manager",
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_Should_Invalidate_Previous_Active_Tokens()
    {
        await using var context = GetInMemoryDbContext();
        var user = new User
        {
            Id = 1,
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "old-hash",
            IsActive = true
        };
        var previousToken = new PasswordResetToken
        {
            Id = 1,
            User = user,
            TokenHash = "previous-hash",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        context.Users.Add(user);
        context.PasswordResetTokens.Add(previousToken);
        await context.SaveChangesAsync();

        var result = await CreateService(context).RequestPasswordResetAsync(1);

        result.Data.Should().BeTrue();
        (await context.PasswordResetTokens.FindAsync(1))!.UsedAt.Should().NotBeNull();
        (await context.PasswordResetTokens.CountAsync()).Should().Be(2);
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Return_Error_For_Invalid_Base64_Token()
    {
        await using var context = GetInMemoryDbContext();

        var result = await CreateService(context)
            .ResetPasswordAsync(new ResetPasswordDto("invalid-token", "new-password"));

        result.Errors.Should().ContainSingle().Which.Should().Be("Token inválido.");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Return_Error_When_Token_Does_Not_Exist()
    {
        await using var context = GetInMemoryDbContext();
        string token = Convert.ToBase64String(new byte[] { 7, 8, 9 });

        var result = await CreateService(context)
            .ResetPasswordAsync(new ResetPasswordDto(token, "new-password"));

        result.Errors.Should().ContainSingle().Which.Should().Be("Token inválido ou expirado.");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Return_Error_When_Token_Was_Already_Used()
    {
        await using var context = GetInMemoryDbContext();
        var user = new User
        {
            Id = 1,
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "old-hash"
        };
        string token = Convert.ToBase64String(new byte[] { 10, 11, 12 });
        context.Users.Add(user);
        context.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = 1,
            User = user,
            TokenHash = HashToken(token),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            UsedAt = DateTime.UtcNow.AddMinutes(-1)
        });
        await context.SaveChangesAsync();

        var result = await CreateService(context)
            .ResetPasswordAsync(new ResetPasswordDto(token, "new-password"));

        result.Errors.Should().ContainSingle().Which.Should().Be("Token inválido ou expirado.");
        (await context.Users.FindAsync(1))!.PasswordHash.Should().Be("old-hash");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Return_Error_For_Expired_Token()
    {
        await using var context = GetInMemoryDbContext();
        var user = new User
        {
            Id = 1,
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "old-hash"
        };
        string token = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        context.Users.Add(user);
        context.PasswordResetTokens.Add(new PasswordResetToken
        {
            User = user,
            TokenHash = HashToken(token),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
        });
        await context.SaveChangesAsync();

        var result = await CreateService(context)
            .ResetPasswordAsync(new ResetPasswordDto(token, "new-password"));

        result.Errors.Should().ContainSingle().Which.Should().Be("Token inválido ou expirado.");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Update_Password_And_Invalidate_Other_Tokens()
    {
        await using var context = GetInMemoryDbContext();
        var user = new User
        {
            Id = 1,
            UserName = "User",
            Email = "user@test.com",
            PasswordHash = "old-hash"
        };
        string token = Convert.ToBase64String(new byte[] { 4, 5, 6 });
        var resetToken = new PasswordResetToken
        {
            Id = 1,
            User = user,
            TokenHash = HashToken(token),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        var otherToken = new PasswordResetToken
        {
            Id = 2,
            User = user,
            TokenHash = "other-hash",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        context.Users.Add(user);
        context.PasswordResetTokens.AddRange(resetToken, otherToken);
        await context.SaveChangesAsync();

        var result = await CreateService(context)
            .ResetPasswordAsync(new ResetPasswordDto(token, "new-password"));

        result.Data.Should().BeTrue();
        result.Message.Should().Be("Senha redefinida com sucesso.");
        var savedUser = await context.Users.FindAsync(1);
        BCrypt.Net.BCrypt.Verify("new-password", savedUser!.PasswordHash).Should().BeTrue();
        (await context.PasswordResetTokens.FindAsync(1))!.UsedAt.Should().NotBeNull();
        (await context.PasswordResetTokens.FindAsync(2))!.UsedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Not_Invalidate_Tokens_From_Other_Users()
    {
        await using var context = GetInMemoryDbContext();
        var userA = new User
        {
            Id = 1,
            UserName = "User A",
            Email = "user-a@test.com",
            PasswordHash = "old-hash-a"
        };
        var userB = new User
        {
            Id = 2,
            UserName = "User B",
            Email = "user-b@test.com",
            PasswordHash = "old-hash-b"
        };
        string tokenA = Convert.ToBase64String(new byte[] { 13, 14, 15 });
        var tokenB = new PasswordResetToken
        {
            Id = 2,
            User = userB,
            TokenHash = "user-b-token-hash",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        context.Users.AddRange(userA, userB);
        context.PasswordResetTokens.AddRange(
            new PasswordResetToken
            {
                Id = 1,
                User = userA,
                TokenHash = HashToken(tokenA),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            },
            tokenB);
        await context.SaveChangesAsync();

        var result = await CreateService(context)
            .ResetPasswordAsync(new ResetPasswordDto(tokenA, "new-password"));

        result.Data.Should().BeTrue();
        (await context.PasswordResetTokens.FindAsync(2))!.UsedAt.Should().BeNull();
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(
            SHA256.HashData(Convert.FromBase64String(token)));
    }
}
