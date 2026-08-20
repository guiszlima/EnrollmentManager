using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Models;

[Index(nameof(Code), IsUnique = true)]

//Class representing the status of an enrollment (e.g., Pending, Approved, Rejected)
public class EnrollmentStatus
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = [];
}