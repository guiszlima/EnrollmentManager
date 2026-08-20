namespace EnrollmentManager.API.DTOs.Common;

public record ApiResponseDto<T>(
    T? Data = default,
    string? Message = null,
    IReadOnlyList<string>? Errors = null
);