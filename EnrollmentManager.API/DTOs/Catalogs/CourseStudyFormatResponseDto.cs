namespace EnrollmentManager.API.DTOs.Catalogs;

public class CourseStudyFormatResponseDto
{
    public int CourseId { get; init; }
    public string CourseName { get; init; } = string.Empty;
    public int FormatId { get; init; }
    public string FormatName { get; init; } = string.Empty;
}