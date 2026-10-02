using System;

namespace ProAspNetCore9.Session02.Endpoint;

public class UpdateTaskRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskItemStatus? Status { get; set; }
}
