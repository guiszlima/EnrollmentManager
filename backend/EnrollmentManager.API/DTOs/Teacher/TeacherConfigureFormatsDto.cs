using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Teacher;

public class TeacherConfigureFormatsDto
{
    [Required]
    [MinLength(1)]
    public List<int> FormatIds { get; set; } = new();
}