

namespace EnrollmentManager.API.DTOs.Common;

public class ApiResponseDto<T>
{
    public T? Data { get; init; }
    public string? Message { get; init; }
   
    public IReadOnlyList<string> Errors { get; init; } =               [];
    public static ApiResponseDto<T> Error(string message)
{
    return new ApiResponseDto<T>
    {
        Errors = [message]
    };
}
}
