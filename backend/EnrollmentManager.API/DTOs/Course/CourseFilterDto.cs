namespace EnrollmentManager.API.DTOs.Course;

public class CourseFilterDto
{
    public int? CourseTypeId { get; set; }
    public int? EducationLevelId { get; set; }
    public int? StatusId { get; set; }
    public List<int> FormatIds { get; set; } = new();
}
