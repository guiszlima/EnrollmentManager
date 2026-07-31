namespace EnrollmentManager.API.DTOs;

public record RegisterUserDTO(
    string UserName,
    string Email,
    string Password
);