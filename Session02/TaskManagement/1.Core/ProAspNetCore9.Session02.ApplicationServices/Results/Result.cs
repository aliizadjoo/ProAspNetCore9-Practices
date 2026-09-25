namespace ProAspNetCore9.Session02.ApplicationServices.Results;

public class Result<T>
{
    public Result(ResultStatus status, T? data)
    {
        Status = status;
        Data = data;
    }

    public ResultStatus Status { get; }

    public T? Data { get; }
}
