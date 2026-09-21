using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Course;

public record CourseInputDto
{
    [Required(ErrorMessage = "Course name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Course name must have between 2 and 100 characters.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "CourseTypeId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CourseTypeId must be greater than zero.")]
    public int CourseTypeId { get; init; }

    [Required(ErrorMessage = "EducationLevelId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "EducationLevelId must be greater than zero.")]
    public int EducationLevelId { get; init; }

    [Required(ErrorMessage = "CourseStatusId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CourseStatusId must be greater than zero.")]
    public int CourseStatusId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "TotalSlots must be greater than zero.")]
    public int TotalSlots { get; init; }

    [MinLength(1, ErrorMessage = "É necessário informar pelo menos um formato de estudo.")]
    public List<int> FormatIds { get; init; } = new();
    public List<int> TeacherIds { get; init; } = new();
}
