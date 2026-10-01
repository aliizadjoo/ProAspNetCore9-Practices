public class TaskItem
{
    private TaskItem()
    {
    }

    private TaskItem(string title, string? description, DateTime? dueDate)
    {
        SetTitle(title);
        Description = description;
        DueDate = dueDate;
        CreatedAt = DateTime.UtcNow;
        Status = TaskItemStatus.New;
    }

    public int Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? DueDate { get; private set; }

    public TaskItemStatus Status { get; private set; }

    public bool IsDeleted { get; private set; }

    public bool IsOverdue =>
        !IsDeleted &&
        Status == TaskItemStatus.New &&
        DueDate.HasValue &&
        DueDate.Value < DateTime.UtcNow;

    public static TaskItem Create(string title, DateTime? dueDate, string? description = null)
    {
        return new TaskItem(title, description, dueDate);
    }

    public void SetTitle(string title)
    {
        EnsureNotDeleted();

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null, empty, or whitespace.", nameof(title));
        }

        Title = title;
    }

    public void SetDescription(string? description)
    {
        EnsureNotDeleted();
        Description = description;
    }

    public void SetDueDate(DateTime? dueDate)
    {
        EnsureNotDeleted();
        DueDate = dueDate;
    }

    public void ChangeStatus(TaskItemStatus status)
    {
        EnsureNotDeleted();

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), status, "Status is not a defined task status.");
        }

        Status = status;
    }

    public void Delete()
    {
        IsDeleted = true;
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("A deleted task cannot be changed.");
        }
    }
}
