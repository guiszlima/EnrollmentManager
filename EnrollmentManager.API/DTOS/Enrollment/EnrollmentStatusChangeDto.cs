using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Enrollment;

public record EnrollmentStatusChangeDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StatusId { get; init; }
}
