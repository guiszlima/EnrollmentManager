using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Enrollment;

public record EnrollmentCreateDTO
{
    [Required(ErrorMessage = "StudentId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "StudentId must be greater than zero.")]
    public int StudentId { get; init; }

    [Required(ErrorMessage = "CourseId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CourseId must be greater than zero.")]
    public int CourseId { get; init; }

    [Required(ErrorMessage = "FormatId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "FormatId must be greater than zero.")]
    public int FormatId { get; init; }
}
