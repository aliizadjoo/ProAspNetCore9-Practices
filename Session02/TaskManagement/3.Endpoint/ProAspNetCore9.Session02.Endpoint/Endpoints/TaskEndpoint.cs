using System.Text.Json;
using System.Text.Json.Serialization;
using ProAspNetCore9.Session02.ApplicationServices.Results;
using ProAspNetCore9.Session02.ApplicationServices.Services;
using ProAspNetCore9.Session02.Endpoint.Models.Requests;
using ProAspNetCore9.Session02.Endpoint.Models.Responses;

namespace ProAspNetCore9.Session02.Endpoint.Endpoints;

public class TaskEndpoint
{
    public const string GetTasksKEY = "GET_TASKS";
    public const string GetTaskByIdKEY = "GET_TASK_BY_ID";
    public const string CreateTaskKEY = "POST_TASKS";
    public const string EditTaskKEY = "PUT_TASK_BY_ID";
    public const string DeleteTaskKEY = "DELETE_TASK_BY_ID";

    public Dictionary<string, Func<HttpContext, Task>> MyRouter = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly TaskAppService _taskAppService;

    public TaskEndpoint(TaskAppService taskAppService)
    {
        _taskAppService = taskAppService;

        MyRouter[GetTasksKEY] = GetTasks;
        MyRouter[GetTaskByIdKEY] = GetTaskById;
        MyRouter[CreateTaskKEY] = CreateTask;
        MyRouter[EditTaskKEY] = EditTask;
        MyRouter[DeleteTaskKEY] = DeleteTask;
    }

    public async Task GetTasks(HttpContext context)
    {
        DateTime? createdDate = null;
        DateTime? dueDate = null;

        var createdDateValue = context.Request.Query["createdDate"];
        if (createdDateValue.Count > 0)
        {
            if (!DateTime.TryParse(createdDateValue.ToString(), out var parsedCreatedDate))
            {
                await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                    new ApiResponse<object>(false, "تاریخ createdDate معتبر نیست.", null));
                return;
            }

            createdDate = parsedCreatedDate;
        }

        var dueDateValue = context.Request.Query["dueDate"];
        if (dueDateValue.Count > 0)
        {
            if (!DateTime.TryParse(dueDateValue.ToString(), out var parsedDueDate))
            {
                await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                    new ApiResponse<object>(false, "تاریخ dueDate معتبر نیست.", null));
                return;
            }

            dueDate = parsedDueDate;
        }

        var tasks = await _taskAppService.GetFilteredAsync(
            createdDate, dueDate, context.RequestAborted);

        await WriteResponseAsync(context, StatusCodes.Status200OK,
            new ApiResponse<IReadOnlyList<TaskItem>>(true, "فهرست وظایف دریافت شد.", tasks));
    }

    public async Task GetTaskById(HttpContext context)
    {
        if (!TryGetTaskId(context, out var id))
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "شناسهٔ وظیفه معتبر نیست.", null));
            return;
        }

        var taskItem = await _taskAppService.GetByIdAsync(id, context.RequestAborted);
        if (taskItem is null)
        {
            await WriteResponseAsync(context, StatusCodes.Status404NotFound,
                new ApiResponse<TaskItem>(false, "وظیفه‌ای با این شناسه پیدا نشد.", null));
            return;
        }

        await WriteResponseAsync(context, StatusCodes.Status200OK,
            new ApiResponse<TaskItem>(true, "وظیفه با موفقیت دریافت شد.", taskItem));
    }

    public async Task CreateTask(HttpContext context)
    {
        var contentType = context.Request.ContentType?.Split(';', 2)[0].Trim();
        if (!string.Equals(contentType, "application/json", StringComparison.OrdinalIgnoreCase))
        {
            await WriteResponseAsync(context, StatusCodes.Status415UnsupportedMediaType,
                new ApiResponse<object>(false, "نوع محتوای درخواست باید application/json باشد.", null));
            return;
        }

        CreateTaskRequest? request;
        try
        {
            request = await JsonSerializer.DeserializeAsync<CreateTaskRequest>(
                context.Request.Body, JsonOptions, context.RequestAborted);
        }
        catch (JsonException)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "ساختار JSON درخواست معتبر نیست.", null));
            return;
        }

        if (request?.Title is null || request.DueDate is null)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "فیلدهای Title و DueDate الزامی هستند.", null));
            return;
        }

        try
        {
            var result = await _taskAppService.CreateAsync(
                request.Title, request.DueDate.Value, request.Description, context.RequestAborted);

            await WriteResponseAsync(context, StatusCodes.Status201Created,
                new ApiResponse<TaskItem>(true, "وظیفه با موفقیت ایجاد شد.", result.Data));
        }
        catch (ArgumentException)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "اطلاعات ورودی معتبر نیست.", null));
        }
    }

    public async Task EditTask(HttpContext context)
    {
        if (!TryGetTaskId(context, out var id))
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "شناسهٔ وظیفه معتبر نیست.", null));
            return;
        }

        var contentType = context.Request.ContentType?.Split(';', 2)[0].Trim();
        if (!string.Equals(contentType, "application/json", StringComparison.OrdinalIgnoreCase))
        {
            await WriteResponseAsync(context, StatusCodes.Status415UnsupportedMediaType,
                new ApiResponse<object>(false, "نوع محتوای درخواست باید application/json باشد.", null));
            return;
        }

        EditTaskRequest? request;
        try
        {
            request = await JsonSerializer.DeserializeAsync<EditTaskRequest>(
                context.Request.Body, JsonOptions, context.RequestAborted);
        }
        catch (JsonException)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "ساختار JSON درخواست معتبر نیست.", null));
            return;
        }

        if (request is null)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "بدنهٔ درخواست الزامی است.", null));
            return;
        }

        try
        {
            var result = await _taskAppService.EditAsync(
                id, request.Title, request.Description, request.DueDate,
                request.Status, context.RequestAborted);

            switch (result.Status)
            {
                case ResultStatus.Success:
                    await WriteResponseAsync(context, StatusCodes.Status200OK,
                        new ApiResponse<TaskItem>(true, "وظیفه با موفقیت ویرایش شد.", result.Data));
                    break;
                case ResultStatus.NoChange:
                    await WriteResponseAsync(context, StatusCodes.Status200OK,
                        new ApiResponse<TaskItem>(true, "تغییری اعمال نشد.", result.Data));
                    break;
                case ResultStatus.NotFound:
                    await WriteResponseAsync(context, StatusCodes.Status404NotFound,
                        new ApiResponse<TaskItem>(false, "وظیفه‌ای با این شناسه پیدا نشد.", null));
                    break;
            }
        }
        catch (ArgumentException)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "اطلاعات ورودی معتبر نیست.", null));
        }
    }

    public async Task DeleteTask(HttpContext context)
    {
        if (!TryGetTaskId(context, out var id))
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                new ApiResponse<object>(false, "شناسهٔ وظیفه معتبر نیست.", null));
            return;
        }

        var result = await _taskAppService.DeleteAsync(id, context.RequestAborted);
        if (result.Status == ResultStatus.NotFound)
        {
            await WriteResponseAsync(context, StatusCodes.Status404NotFound,
                new ApiResponse<object>(false, "وظیفه‌ای با این شناسه پیدا نشد.", null));
            return;
        }

        context.Response.StatusCode = StatusCodes.Status204NoContent;
    }

    private static bool TryGetTaskId(HttpContext context, out int id)
    {
        const string prefix = "/tasks/";
        var path = context.Request.Path.Value ?? string.Empty;
        id = 0;

        if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var idValue = path[prefix.Length..];
        return !idValue.Contains('/') && int.TryParse(idValue, out id);
    }

    private static async Task WriteResponseAsync<T>(
        HttpContext context, int statusCode, ApiResponse<T> response)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(
            context.Response.Body, response, JsonOptions, context.RequestAborted);
    }
}
