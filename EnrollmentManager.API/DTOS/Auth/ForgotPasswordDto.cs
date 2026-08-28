using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOS.Auth;

public record ForgotPasswordDto(
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    string Email
);