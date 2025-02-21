namespace Doctor.Communication.Response;

public class Result<T>
{
    public T Data { get; private set; }
    public bool Success { get; private set; }
    public List<string> Errors { get; private set; }

    public bool IsSuccess() => Success;

    public void Succeeded(T data)
    {
        Data = data;
        Success = true;
        Errors = Enumerable.Empty<string>().ToList();
    }

    public void Failure(List<string> errors)
    {
        Errors = errors;
        Data = default;
        Success = false;
    }
}
