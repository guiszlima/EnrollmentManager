namespace EnrollmentManager.API.DTOs.CourseReport;

public class CourseReportEnrollmentDto
{
    public int Id { get; init; }
    public string EnrollmentStatus { get; init; } = string.Empty;
    public DateTime EnrollmentDate { get; init; }
    public CourseReportFormatDto? Format { get; init; }
    public CourseReportStudentDto Student { get; init; } = new();
}
