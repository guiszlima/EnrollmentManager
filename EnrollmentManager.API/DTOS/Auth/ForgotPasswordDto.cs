using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOS.Auth;

public record ForgotPasswordDto(
    [Required]
    [EmailAddress]
    string Email
);