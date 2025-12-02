namespace ExpenseTracker.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public T? Data { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data, string? message = null)
    {
        Success = true;
        Data = data;
        Message = message;
    }

    public static ApiResponse<T> Fail(string message)
        => new ApiResponse<T> { Success = false, Message = message };
}