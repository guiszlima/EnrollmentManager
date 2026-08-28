namespace EnrollmentManager.API.DTOS.Student;

public class StudentResponseDTO
{
    public int UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Cpf { get; init; }
    public string? PassportNumber { get; init; }
    public string? Nationality { get; init; }
    public DateTime BirthDate { get; init; }
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string RegistrationNumber { get; init; } = string.Empty;
}