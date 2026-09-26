using Microsoft.EntityFrameworkCore;
using ProAspNetCore9.Session02.ApplicationServices.Contracts;
using ProAspNetCore9.Session02.ApplicationServices.Services;
using ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Data;
using ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TaskManagement");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'TaskManagement' was not found in configuration.");
}

builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TaskAppService>();

var app = builder.Build();

app.Run();
