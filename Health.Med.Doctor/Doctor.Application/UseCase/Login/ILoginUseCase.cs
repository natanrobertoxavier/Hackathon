using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Login;

public interface ILoginUseCase
{
    Task<Result<ResponseLogin>> LoginAsync(RequestLoginDoctor request);
}
