using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Auth;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace EnrollmentManager.Tests.Services;

public class TokenServiceTests
{
    private static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-secret-key-with-at-least-32-characters",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience"
            })
            .Build();
    }

    [Fact]
    public void GenerateToken_Should_Include_User_Claims_And_Metadata()
    {
        var user = new User
        {
            Id = 7,
            UserName = "Test User",
            Email = "user@test.com",
            Role = new Role { Name = "Student" }
        };

        string token = new TokenService(GetConfiguration()).GenerateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Claims.Should().Contain(claim =>
            claim.Type == JwtRegisteredClaimNames.NameId && claim.Value == "7");
        jwt.Claims.Should().Contain(claim =>
            claim.Type == JwtRegisteredClaimNames.UniqueName && claim.Value == "Test User");
        jwt.Claims.Should().Contain(claim =>
            claim.Type == JwtRegisteredClaimNames.Email && claim.Value == "user@test.com");
        jwt.Claims.Should().Contain(claim =>
            claim.Type == "role" && claim.Value == "Student");
        jwt.Issuer.Should().Be("test-issuer");
        jwt.Audiences.Should().ContainSingle().Which.Should().Be("test-audience");
        jwt.ValidTo.Should().BeAfter(DateTime.UtcNow.AddHours(1));
        jwt.ValidTo.Should().BeBefore(DateTime.UtcNow.AddHours(3));
    }

    [Fact]
    public void GenerateToken_Should_Throw_When_Jwt_Key_Is_Missing()
    {
        var configuration = new ConfigurationBuilder().Build();

        var action = () => new TokenService(configuration).GenerateToken(new User());

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT Key not found in configuration.");
    }
}
