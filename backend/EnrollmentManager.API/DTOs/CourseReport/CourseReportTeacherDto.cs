namespace EnrollmentManager.API.DTOs.CourseReport;

public class CourseReportTeacherDto
{
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public List<CourseReportFormatDto> Formats { get; init; } = new();
}
