using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key] public int Id { get; set; }

    [Required] [MaxLength(100)] public string UserName { get; set; } = string.Empty;

    [Required] public string PasswordHash { get; set; } = string.Empty;

    [Required] [MaxLength(150)] public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "Student";

    public bool Active { get; set; } = false;
}