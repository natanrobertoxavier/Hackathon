namespace Doctor.Persistence.Communication.Response;

public struct Result<T>(
    T value,
    bool isSuccess,
    List<string> errors)
{
    public T Data { get; } = value;
    public bool IsSuccess { get; } = isSuccess;
    public List<string> Errors { get; } = errors;

    public Result<T> Success(T data) => new Result<T>(data, true, Enumerable.Empty<string>().ToList());

    public new Result<T> Failure(List<string> errors) => new Result<T>(default, false, errors);
}
