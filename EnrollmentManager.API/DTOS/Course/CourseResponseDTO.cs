namespace EnrollmentManager.API.DTOs.Course;

public record CourseResponseDTO
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    
    public int CourseTypeId { get; init; }
    public string CourseTypeName { get; init; } = string.Empty;

    public int EducationLevelId { get; init; }
    public string EducationLevelName { get; init; } = string.Empty;

    public int StatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
}