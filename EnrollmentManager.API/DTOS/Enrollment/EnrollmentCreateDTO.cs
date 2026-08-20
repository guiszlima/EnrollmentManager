using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Enrollment;

public record EnrollmentCreateDTO
{
    [Required(ErrorMessage = "StudentId is required.")]
    public int StudentId { get; init; }

    [Required(ErrorMessage = "CourseId is required.")]
    public int CourseId { get; init; }

    [Required(ErrorMessage = "StatusId is required.")]
    public int StatusId { get; init; }

    [Required(ErrorMessage = "FormatId is required.")]
    public int FormatId { get; init; }
}