using Microsoft.EntityFrameworkCore;

namespace ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Data;

public class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> TaskItems { get; set; } = null!;
}
