
using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOS;          
using EnrollmentManager.API.DTOS.Auth;     
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Auth;
using EnrollmentManager.API.Services.Interfaces.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
namespace EnrollmentManager.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;

    public AuthServiceTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _tokenServiceMock = new Mock<ITokenService>();
    }

    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task RegisterAsync_Should_Fail_When_Email_Already_Exists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Users.Add(new User { UserName = "Existing", Email = "test@test.com", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var service = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);
        var dto = new RegisterUserDTO("NewUser", "test@test.com", "Password123");

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("O e-mail já está em uso.");
    }

    [Fact]
    public async Task RegisterAsync_Should_Register_Successfully_When_Data_Is_Valid()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);
        
        var dto = new RegisterUserDTO("NewUser", "new@test.com", "Password123");
        
        _passwordHasherMock
            .Setup(p => p.HashPassword(It.IsAny<User>(), dto.Password))
            .Returns("secure_hashed_password");

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        context.Users.Should().ContainSingle(u => u.Email == "new@test.com");
        var savedUser = await context.Users.FirstAsync(u => u.Email == "new@test.com");
        savedUser.PasswordHash.Should().Be("secure_hashed_password");
        _passwordHasherMock.Verify(
    p => p.HashPassword(It.IsAny<User>(), dto.Password),
    Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Fail_When_User_Not_Found()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);
        var dto = new LoginUserDTO("notfound@test.com", "Password123");

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task LoginAsync_Should_Fail_When_Password_Is_Invalid()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var user = new User { UserName = "User", Email = "user@test.com", PasswordHash = "valid_hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _passwordHasherMock
            .Setup(p => p.VerifyHashedPassword(user, user.PasswordHash, "WrongPassword"))
            .Returns(PasswordVerificationResult.Failed);

        var service = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);
        var dto = new LoginUserDTO("user@test.com", "WrongPassword");

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Token_When_Credentials_Are_Valid()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var user = new User { UserName = "User", Email = "user@test.com", PasswordHash = "valid_hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _passwordHasherMock
            .Setup(p => p.VerifyHashedPassword(user, user.PasswordHash, "Password123"))
            .Returns(PasswordVerificationResult.Success);

        _tokenServiceMock
            .Setup(t => t.GenerateToken(user))            
            .Returns("mocked_jwt_token");

         
        var service = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);
        var dto = new LoginUserDTO("user@test.com", "Password123");

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be("mocked_jwt_token");
        result.Message.Should().Be("Login realizado com sucesso.");

           _tokenServiceMock.Verify(
            t => t.GenerateToken(user),
            Times.Once);

    }
}