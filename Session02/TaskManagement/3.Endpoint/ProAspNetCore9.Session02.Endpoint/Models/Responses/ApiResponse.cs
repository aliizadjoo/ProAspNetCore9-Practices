namespace ProAspNetCore9.Session02.Endpoint.Models.Responses;

public sealed class ApiResponse<T>(bool success, string message, T? data)
{
    public bool Success { get; } = success;

    public string Message { get; } = message;

    public T? Data { get; } = data;
}
