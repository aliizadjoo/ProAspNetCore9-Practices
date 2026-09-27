namespace ProAspNetCore9.Session02.Endpoint.Models.Requests;

public sealed class CreateTaskRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }
}
