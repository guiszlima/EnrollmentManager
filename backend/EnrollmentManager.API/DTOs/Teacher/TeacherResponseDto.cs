namespace EnrollmentManager.API.DTOs.Teacher;

public class TeacherResponseDto
{
    public int UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public List<int> FormatIds { get; init; } = new();
}
