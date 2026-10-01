using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProAspNetCore9.Session02.ApplicationServices.Contracts;
using ProAspNetCore9.Session02.ApplicationServices.Services;
using ProAspNetCore9.Session02.Endpoint;
using ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Data;
using ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TaskManagement");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'TaskManagement' was not found in configuration.");
}

builder.Services.AddDbContext<TaskManagementDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TaskAppService>();

var app = builder.Build();

app.Run(async (context) =>
{

    if (context.Request.Method == "GET" && context.Request.Path == "/tasks")
    {
        DateTime? createdDate = null;
        DateTime? dueDate = null;
        if (context.Request.Query.TryGetValue("createdDate", out var createdDateValue))
        {
            if (DateTime.TryParse(createdDateValue.ToString(), out var parsedCreatedDate))
            {
                createdDate = parsedCreatedDate;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "createdDate is invalid."
                });

                return;
            }
        }

        if (context.Request.Query.TryGetValue("dueDate", out var dueDateValue))
        {
            if (DateTime.TryParse(dueDateValue.ToString(), out var parseddueDate))
            {
                dueDate = parseddueDate;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "dueDate is invalid"
                });

                return;

            }
        }

        var taskAppService = context.RequestServices.GetRequiredService<TaskAppService>();

        var tasks = await taskAppService.GetFilteredAsync(createdDate, dueDate, context.RequestAborted);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(tasks);

        return;

    }

    if (context.Request.Method == "GET")
    {
        var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments is { Length: 2 } && segments[0] == "tasks")
        {
            if (!int.TryParse(segments[1], out var id))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Task id is invalid."
                });

                return;
            }
            var taskAppService = context.RequestServices.GetRequiredService<TaskAppService>();
            var task = await taskAppService.GetByIdAsync(id, context.RequestAborted);

            if (task == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Task not found."
                });

                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(task);

            return;
        }
    }

    if (context.Request.Method == "POST" && context.Request.Path == "/tasks")
    {
        //همین سه مرحله بخش دستی Model Binding ما هستند: Request.Body یک Stream است، با StreamReader خوانده می‌شود و متن JSON با JsonSerializer.Deserialize به مدل تبدیل می‌شود.
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        CreateTaskRequest? createTaskRequest;
        try
        {
            createTaskRequest = JsonSerializer.Deserialize<CreateTaskRequest>(body);
        }
        catch (JsonException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Request body contains invalid JSON."
            });
            return;
        }

        if (createTaskRequest == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Request body cannot be null."
            });

            return;
        }
        if (string.IsNullOrWhiteSpace(createTaskRequest.Title))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Title is required."
            });
            return;
        }

        var taskAppService = context.RequestServices.GetRequiredService<TaskAppService>();
        try
        {
            var taskResult = await taskAppService.CreateAsync(createTaskRequest.Title, createTaskRequest.DueDate, createTaskRequest.Description, context.RequestAborted);

            context.Response.StatusCode = StatusCodes.Status201Created;
            context.Response.ContentType = "application/json";
            context.Response.Headers["Location"] =
             $"/tasks/{taskResult.Data.Id}";

            await context.Response.WriteAsJsonAsync(taskResult.Data);
            return;


        }
        catch (InvalidOperationException)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                message = "An error occurred while creating the task."
            });

            return;

        }
    }
});



app.Run();
