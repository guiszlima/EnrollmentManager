using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOS;

public record LoginUserDto(
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    string Email,
    [Required(ErrorMessage = "A senha é obrigatória.")]
    string Password
);