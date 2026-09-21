namespace EnrollmentManager.API.DTOs.CourseReport;

public class CourseReportDto
{
    public CourseReportCourseDto Course { get; init; } = new();
    public List<CourseReportEnrollmentDto> Enrollments { get; init; } = new();
    public List<CourseReportTeacherDto> Teachers { get; init; } = new();
}
