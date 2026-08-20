using System.Security.Cryptography;
using EnrollmentManager.API.Configurations;
using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOS.Auth;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Interfaces;
using EnrollmentManager.API.Services.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnrollmentManager.API.Services.Auth;

public class PasswordResetService : IPasswordResetService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly AppConfiguration _appConfiguration;

    public PasswordResetService(
        ApplicationDbContext context,
        IEmailService emailService,
        IOptions<AppConfiguration> appConfiguration)
    {
        _context = context;
        _emailService = emailService;
        _appConfiguration = appConfiguration.Value;
    }

    public async Task<ApiResponseDto<bool>> RequestPasswordResetAsync(
        int userId)
    {
     

        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return new ApiResponseDto<bool>(Errors: ["Usuário não encontrado"]);
        }

        // Invalida tokens anteriores que ainda poderiam ser utilizados.
        List<PasswordResetToken> activeTokens =
            await _context.PasswordResetTokens
                .Where(t =>
                    t.UserId == user.Id &&
                    t.UsedAt == null &&
                    t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

        foreach (PasswordResetToken token in activeTokens)
        {
            token.UsedAt = DateTime.UtcNow;
        }

        TokenResult tokenResult = GenerateToken();

        PasswordResetToken resetToken = new()
        {
            UserId = user.Id,
            TokenHash = tokenResult.TokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        _context.PasswordResetTokens.Add(resetToken);

        await _context.SaveChangesAsync();

        string resetLink =
            $"{_appConfiguration.FrontendUrl}/reset-password?token={Uri.EscapeDataString(tokenResult.Token)}";

        string body = BuildPasswordResetEmail(resetLink);

        await _emailService.SendAsync(
            user.Email,
            "Redefinição de senha - Enrollment Manager",
            body
        );

        return new ApiResponseDto<bool>(
            Data: true,
            Message: "Instruções para redefinição de senha enviadas ao usuário."
        );
    }

    public async Task<ApiResponseDto<bool>> ResetPasswordAsync(
        ResetPasswordDto dto)
    {
        byte[] tokenBytes;

        try
        {
            tokenBytes = Convert.FromBase64String(dto.Token);
        }
        catch (FormatException)
        {
            return new ApiResponseDto<bool>(Errors: ["Token inválido."]);
        }

        string tokenHash = Convert.ToHexString(
            SHA256.HashData(tokenBytes)
        );

        PasswordResetToken? resetToken =
            await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.TokenHash == tokenHash &&
                    t.UsedAt == null &&
                    t.ExpiresAt > DateTime.UtcNow
                );

        if (resetToken is null)
        {
            return new ApiResponseDto<bool>(Errors: ["Token inválido ou expirado."]);
        }

        resetToken.User.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        // Marca o token utilizado como consumido.
        resetToken.UsedAt = DateTime.UtcNow;

        // Invalida qualquer outro token que eventualmente ainda esteja ativo.
        List<PasswordResetToken> otherActiveTokens =
            await _context.PasswordResetTokens
                .Where(t =>
                    t.UserId == resetToken.UserId &&
                    t.Id != resetToken.Id &&
                    t.UsedAt == null &&
                    t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

        foreach (PasswordResetToken token in otherActiveTokens)
        {
            token.UsedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return new ApiResponseDto<bool>(
            Data: true,
            Message: "Senha redefinida com sucesso."
        );
    }

    private static TokenResult GenerateToken()
    {
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);

        string token = Convert.ToBase64String(tokenBytes);

        string tokenHash = Convert.ToHexString(
            SHA256.HashData(tokenBytes)
        );

        return new TokenResult(
            token,
            tokenHash
        );
    }

    private static string BuildPasswordResetEmail(string resetLink)
    {
        return $"""
                <!DOCTYPE html>
                <html lang="pt-BR">
                <body style="margin:0; padding:0; background-color:#f4f4f5; font-family:Arial,sans-serif;">
                    <div style="max-width:600px; margin:40px auto; background:#ffffff; padding:40px; border-radius:12px;">
                        
                        <h1 style="margin-top:0; color:#18181b;">
                            Enrollment Manager
                        </h1>

                        <p style="font-size:16px; color:#3f3f46;">
                            Você solicitou a redefinição da sua senha.
                        </p>

                        <p style="font-size:16px; color:#3f3f46;">
                            Clique no botão abaixo para criar uma nova senha:
                        </p>

                        <div style="text-align:center; margin:32px 0;">
                            <a href="{resetLink}"
                               style="display:inline-block; padding:14px 24px;
                                      background:#18181b; color:#ffffff;
                                      text-decoration:none; border-radius:8px;
                                      font-weight:bold;">
                                Redefinir minha senha
                            </a>
                        </div>

                        <p style="font-size:14px; color:#71717a;">
                            Este link é válido por 30 minutos.
                        </p>

                        <p style="font-size:14px; color:#71717a;">
                            Se você não solicitou essa alteração, ignore este e-mail.
                        </p>

                        <hr style="border:none; border-top:1px solid #e4e4e7; margin:32px 0;">

                        <p style="font-size:12px; color:#a1a1aa;">
                            Enrollment Manager
                        </p>
                    </div>
                </body>
                </html>
                """;
    }

    private record TokenResult(
        string Token,
        string TokenHash
    );
}