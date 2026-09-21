namespace EnrollmentManager.API.DTOs.Course;

public record CourseResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    
    public int CourseTypeId { get; init; }
    public string CourseTypeName { get; init; } = string.Empty;

    public int EducationLevelId { get; init; }
    public string EducationLevelName { get; init; } = string.Empty;

    public int StatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public List<int> FormatIds { get; init; } = new();
    public List<int> TeacherIds { get; init; } = new();
    public int TotalSlots { get; init; }
    public int AvailableSlots { get; init; }
    public int EnrollmentCount { get; init; }
    public List<CourseEnrollmentDto> Enrollments { get; init; } = new();
}

public class CourseEnrollmentDto
{
    public int Id { get; init; }
    public int StudentId { get; init; }
    public string StudentName { get; init; } = string.Empty;
    public string StudentEmail { get; init; } = string.Empty;
    public string RegistrationNumber { get; init; } = string.Empty;
    public int FormatId { get; init; }
    public string FormatName { get; init; } = string.Empty;
    public int StatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public DateTime EnrollmentDate { get; init; }
}
