using Microsoft.EntityFrameworkCore;
using ProAspNetCore9.Session02.ApplicationServices.Contracts;
using ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Data;

namespace ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        await _context.TaskItems.AddAsync(taskItem, cancellationToken);
    }

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _context.TaskItems
            .FirstOrDefaultAsync(
                task => task.Id == id && !task.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetFilteredAsync(
        DateTime? createdDate,
        DateTime? dueDate,
        CancellationToken cancellationToken)
    {
        var query = _context.TaskItems
            .AsNoTracking()
            .Where(task => !task.IsDeleted);

        if (createdDate.HasValue)
        {
            var start = createdDate.Value.Date;
            var end = start.AddDays(1);
            query = query.Where(task => task.CreatedAt >= start && task.CreatedAt < end);
        }

        if (dueDate.HasValue)
        {
            var start = dueDate.Value.Date;
            var end = start.AddDays(1);
            query = query.Where(task => task.DueDate >= start && task.DueDate < end);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
