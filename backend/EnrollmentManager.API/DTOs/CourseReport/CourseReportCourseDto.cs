namespace EnrollmentManager.API.DTOs.CourseReport;

public class CourseReportCourseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CourseTypeName { get; init; } = string.Empty;
    public string EducationLevelName { get; init; } = string.Empty;
    public string StatusName { get; init; } = string.Empty;
    public int TotalSlots { get; init; }
    public int AvailableSlots { get; init; }
    public int EnrollmentCount { get; init; }
    public List<CourseReportFormatDto> Formats { get; init; } = new();
}
