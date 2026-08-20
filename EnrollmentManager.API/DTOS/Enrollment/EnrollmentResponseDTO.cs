namespace EnrollmentManager.API.DTOs.Enrollment;

public record EnrollmentResponseDTO
{
    public int Id { get; init; }

    public int StudentId { get; init; }
    public string StudentName { get; init; } = string.Empty;

    public int CourseId { get; init; }
    public string CourseName { get; init; } = string.Empty;

    public int StatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;

    public int FormatId { get; init; }
    public string FormatName { get; init; } = string.Empty;

    public DateTime EnrollmentDate { get; init; }
    public DateTime? CompletionDate { get; init; }
}