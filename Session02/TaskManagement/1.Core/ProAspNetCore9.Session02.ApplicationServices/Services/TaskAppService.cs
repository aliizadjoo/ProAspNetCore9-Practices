using ProAspNetCore9.Session02.ApplicationServices.Contracts;
using ProAspNetCore9.Session02.ApplicationServices.Results;

namespace ProAspNetCore9.Session02.ApplicationServices.Services;

public class TaskAppService
{
    private readonly ITaskRepository _taskRepository;

    public TaskAppService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<TaskItem>> CreateAsync(
        string title,
        DateTime? dueDate,
        string? description,
        CancellationToken cancellationToken)
    {
        var taskItem = TaskItem.Create(title, dueDate, description);

        await _taskRepository.AddAsync(taskItem, cancellationToken);

        var affectedRows = await _taskRepository.SaveChangesAsync(cancellationToken);
        if (affectedRows == 0)
        {
            throw new InvalidOperationException("Creating a task did not save any changes.");
        }

        return new Result<TaskItem>(ResultStatus.Success, taskItem);
    }

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _taskRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<TaskItem>> GetFilteredAsync(
        DateTime? createdDate,
        DateTime? dueDate,
        CancellationToken cancellationToken)
    {
        return _taskRepository.GetFilteredAsync(createdDate, dueDate, cancellationToken);
    }

    public async Task<Result<TaskItem>> EditAsync(
        int id,
        string? title,
        string? description,
        DateTime? dueDate,
        TaskItemStatus? status,
        CancellationToken cancellationToken)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (taskItem is null)
        {
            return new Result<TaskItem>(ResultStatus.NotFound, null);
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            taskItem.SetTitle(title);
        }

        if (description is not null)
        {
            taskItem.SetDescription(description);
        }

        if (dueDate.HasValue)
        {
            taskItem.SetDueDate(dueDate.Value);
        }

        if (status.HasValue)
        {
            taskItem.ChangeStatus(status.Value);
        }

        var affectedRows = await _taskRepository.SaveChangesAsync(cancellationToken);
        var resultStatus = affectedRows > 0 ? ResultStatus.Success : ResultStatus.NoChange;

        return new Result<TaskItem>(resultStatus, taskItem);
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (taskItem is null)
        {
            return new Result<bool>(ResultStatus.NotFound, false);
        }

        taskItem.Delete();

        var affectedRows = await _taskRepository.SaveChangesAsync(cancellationToken);
        if (affectedRows == 0)
        {
            throw new InvalidOperationException("Deleting a task did not save any changes.");
        }

        return new Result<bool>(ResultStatus.Success, true);
    }
}
