using Newtonsoft.Json;

namespace Consultation.Domain.ModelServices;

public class Result<T>
{
    [JsonProperty("data")]
    public T Data { get; set; } = default;

    [JsonProperty("success")]
    public bool Success { get; set; } = false;

    [JsonProperty("error")]
    public string Error { get; set; } = string.Empty;

    public Result() { }

    public Result(
        T data,
        bool isSuccess,
        string error)
    {
        Data = data;
        Success = isSuccess;
        Error = error;
    }

    public Result<T> Succeeded(T data) => new Result<T>(data, true, string.Empty);

    public new Result<T> Failure(string error) => new Result<T>(default, false, error);
}
