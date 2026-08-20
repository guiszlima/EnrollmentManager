using EnrollmentManager.API.DTOS;
using EnrollmentManager.API.DTOS.Auth;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Identity; 
using EnrollmentManager.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EnrollmentManager.API.DTOs.Common;

namespace EnrollmentManager.API.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    // Injete o IPasswordHasher via construtor
    public AuthService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher , ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<ApiResponseDto<string>> RegisterAsync(RegisterUserDto dto)
    {

        bool emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
        {
            return new ApiResponseDto<string>(Errors: ["O e-mail já está em uso."]);
        }
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            
           
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new ApiResponseDto<string>(Message: "Usuário registrado com sucesso.");
    }

  public async Task<ApiResponseDto<string>> LoginAsync(LoginUserDto dto)
    {
        // 1. Busca o usuário completo pelo e-mail
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
        {
            return new ApiResponseDto<string>(Errors: ["Credenciais inválidas."]);
        }

        // 2. Valida a senha usando o PasswordHasher
        var resultado = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (resultado == PasswordVerificationResult.Failed)
        {
            return new ApiResponseDto<string>(Errors: ["Credenciais inválidas."]);
        }

        // 3. Gera o token JWT usando a classe separada
        string token = _tokenService.GenerateToken(user);

        return new ApiResponseDto<string>(
            Data: token,
            Message: "Login realizado com sucesso."
        );
    }
}