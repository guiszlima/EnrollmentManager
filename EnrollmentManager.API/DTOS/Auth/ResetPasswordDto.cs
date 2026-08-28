using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOS.Auth;

public record ResetPasswordDto(
    [Required(ErrorMessage = "O token é obrigatório.")]
    [MinLength(1, ErrorMessage = "O token é obrigatório.")]
    string Token,

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A nova senha deve ter entre 6 e 100 caracteres.")]
    string NewPassword
);