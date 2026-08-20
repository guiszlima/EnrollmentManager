using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOS.Auth;

public record RegisterUserDto(
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    string UserName,
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    string Email,
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    string Password
);