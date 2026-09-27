using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProAspNetCore9.Session02.ApplicationServices.Contracts;
using ProAspNetCore9.Session02.ApplicationServices.Services;
using ProAspNetCore9.Session02.Endpoint.Endpoints;
using ProAspNetCore9.Session02.Endpoint.Models.Responses;
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
builder.Services.AddScoped<TaskEndpoint>();

var app = builder.Build();

app.Run(async context =>
{
    try
    {
        var taskEndpoint = context.RequestServices.GetRequiredService<TaskEndpoint>();
        var path = context.Request.Path.Value ?? string.Empty;
        string? key = null;

        if (string.Equals(path, "/tasks", StringComparison.OrdinalIgnoreCase))
        {
            if (HttpMethods.IsGet(context.Request.Method))
            {
                key = TaskEndpoint.GetTasksKEY;
            }
            else if (HttpMethods.IsPost(context.Request.Method))
            {
                key = TaskEndpoint.CreateTaskKEY;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                context.Response.Headers.Allow = "GET, POST";
                return;
            }
        }
        else if (path.StartsWith("/tasks/", StringComparison.OrdinalIgnoreCase))
        {
            var idValue = path["/tasks/".Length..];
            if (idValue.Length == 0 || idValue.Contains('/'))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            if (HttpMethods.IsGet(context.Request.Method))
            {
                key = TaskEndpoint.GetTaskByIdKEY;
            }
            else if (HttpMethods.IsPut(context.Request.Method))
            {
                key = TaskEndpoint.EditTaskKEY;
            }
            else if (HttpMethods.IsDelete(context.Request.Method))
            {
                key = TaskEndpoint.DeleteTaskKEY;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                context.Response.Headers.Allow = "GET, PUT, DELETE";
                return;
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        if (taskEndpoint.MyRouter.TryGetValue(key, out var handler))
        {
            await handler.Invoke(context);
        }
    }
    catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
    {
        // Request cancellation is normal when the client disconnects.
    }
    catch (Exception)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                false,
                "خطای غیرمنتظره‌ای در سرور رخ داد.",
                null);

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                response,
                cancellationToken: context.RequestAborted);
        }
    }
});

app.Run();
