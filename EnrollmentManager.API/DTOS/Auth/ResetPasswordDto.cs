using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOS.Auth;

public record ResetPasswordDto(
    [Required]
    string Token,

    [Required]
    [MinLength(6)]
    string NewPassword
);