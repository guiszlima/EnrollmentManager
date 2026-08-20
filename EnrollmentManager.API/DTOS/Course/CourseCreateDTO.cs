using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Course;

public record CourseCreateDTO
{
    [Required(ErrorMessage = "Course name is required.")]
    [MaxLength(100, ErrorMessage = "Course name cannot exceed 100 characters.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "CourseTypeId is required.")]
    public int CourseTypeId { get; init; }

    [Required(ErrorMessage = "EducationLevelId is required.")]
    public int EducationLevelId { get; init; }

    [Required(ErrorMessage = "StatusId is required.")]
    public int StatusId { get; init; }
}