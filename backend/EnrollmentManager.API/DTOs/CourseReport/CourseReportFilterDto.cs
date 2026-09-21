namespace EnrollmentManager.API.DTOs.CourseReport;

public class CourseReportFilterDto
{
    public List<int> EnrollmentStatusIds { get; set; } = new();
    public List<int> FormatIds { get; set; } = new();
    public List<int> TeacherIds { get; set; } = new();
}
