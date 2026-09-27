namespace ProAspNetCore9.Session02.Endpoint.Models.Requests;

public sealed class EditTaskRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskItemStatus? Status { get; set; }
}
