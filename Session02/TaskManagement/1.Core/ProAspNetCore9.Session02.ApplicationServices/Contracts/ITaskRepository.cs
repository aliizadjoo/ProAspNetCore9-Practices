namespace ProAspNetCore9.Session02.ApplicationServices.Contracts;

public interface ITaskRepository
{
    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken);

    Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItem>> GetFilteredAsync(
        DateTime? createdDate,
        DateTime? dueDate,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
