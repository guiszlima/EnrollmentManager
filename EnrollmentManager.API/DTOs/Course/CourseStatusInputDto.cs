using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Course;

public record CourseStatusInputDto
{
    [Required(ErrorMessage = "Course status name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Course status name must have between 2 and 50 characters.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Course status code is required.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Course status code must have between 2 and 30 characters.")]
    public string Code { get; init; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Course status description cannot exceed 200 characters.")]
    public string? Description { get; init; }
}