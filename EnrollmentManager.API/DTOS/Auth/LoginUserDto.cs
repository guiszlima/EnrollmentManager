using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOS;

public record LoginUserDto(
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    string Email,
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    [MaxLength(100, ErrorMessage = "A senha não pode exceder 100 caracteres.")]
    string Password
);