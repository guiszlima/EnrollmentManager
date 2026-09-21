namespace EnrollmentManager.API.DTOs.CourseReport;

public class CourseReportStudentDto
{
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string RegistrationNumber { get; init; } = string.Empty;
    public string? Nationality { get; init; }
    public string Phone { get; init; } = string.Empty;
}
