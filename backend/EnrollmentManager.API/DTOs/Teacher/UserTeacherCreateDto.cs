using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Teacher;

public class UserTeacherCreateDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<int> FormatIds { get; set; } = new();
}
