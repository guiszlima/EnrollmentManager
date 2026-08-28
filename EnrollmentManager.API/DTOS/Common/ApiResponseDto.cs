

namespace EnrollmentManager.API.DTOs.Common;

public class ApiResponseDto<T>
{
    public T? Data { get; init; } = default;
    public string? Message { get; init; } = null;
   
    public IReadOnlyList<string> Errors { get; init; } =               [];
    public static ApiResponseDto<T> Error(string message)
{
    return new ApiResponseDto<T>
    {
        Errors = [message]
    };
}
}
