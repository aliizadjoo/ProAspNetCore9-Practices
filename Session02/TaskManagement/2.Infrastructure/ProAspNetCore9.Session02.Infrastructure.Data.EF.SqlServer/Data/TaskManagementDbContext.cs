using Microsoft.EntityFrameworkCore;

namespace ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Data;

public class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> TaskItems { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<TaskItem>().HasData(
        new
        {
            Id = 1,
            Title = "Learn ASP.NET Core",
            Description = "Practice manual routing and HttpContext",
            CreatedAt = new DateTime(2026, 9, 28, 10, 0, 0),
            DueDate = new DateTime(2026, 10, 2),
            Status = TaskItemStatus.New,
            IsDeleted = false
        },
        new
        {
            Id = 2,
            Title = "Learn EF Core",
            Description = "Practice DbContext and repository",
            CreatedAt = new DateTime(2026, 9, 29, 11, 0, 0),
            DueDate = new DateTime(2026, 10, 5),
            Status = TaskItemStatus.Done,
            IsDeleted = false
        },
        new
        {
            Id = 3,
            Title = "Test GET endpoint",
            Description = "Test endpoint using Postman",
            CreatedAt = new DateTime(2026, 10, 1, 9, 0, 0),
            DueDate = new DateTime(2026, 10, 7),
            Status = TaskItemStatus.New,
            IsDeleted = false
        },
        new
        {
            Id = 4,
            Title = "Refactor Task Management",
            Description = "Review architecture and dependencies",
            CreatedAt = new DateTime(2026, 10, 1, 12, 0, 0),
            DueDate = new DateTime(2026, 10, 10),
            Status = TaskItemStatus.Canceled,
            IsDeleted = false
        }
    );
}
}
