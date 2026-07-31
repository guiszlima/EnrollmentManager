namespace EnrollmentManager.API.DTOS;

public record ApiResponseDTO<T>(
    bool Success,
    string? Message = null,
    T? Data = default,
    IReadOnlyList<string>? Errors = null
)
{
    
    public static ApiResponseDTO<T> Ok(T data, string? message = null) 
        => new(true, message, data);

    public static ApiResponseDTO<T> Fail(string message, IReadOnlyList<string>? errors = null) 
        => new(false, message, default, errors ?? [])
     ;
        
}