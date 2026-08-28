using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOS.Auth;

public record RegisterUserDto(
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 100 caracteres.")]
    string UserName,
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    string Email,
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
    string Password
);