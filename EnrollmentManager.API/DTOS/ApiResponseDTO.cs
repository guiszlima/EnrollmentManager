namespace EnrollmentManager.API.DTOs;

public record ApiResponse<T>(
    bool Success,
    string? Message = null,
    T? Data = default,
    List<string>? Errors = null
)
{
    // Métodos estáticos de conveniência continuam funcionando normalmente
    public static ApiResponse<T> Ok(T data, string? message = null) 
        => new(true, message, data);

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) 
        => new(false, message, default, errors ?? [])
     ;
        
}