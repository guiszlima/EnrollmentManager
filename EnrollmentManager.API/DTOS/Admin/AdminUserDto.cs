using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.Dtos.User;

public record AdminUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Status { get; set; }
    
}