using Client.Communication.Request;
using Client.Communication.Response;

namespace Client.Application.UseCase.Login;
public interface ILoginUseCase
{
    Task<Result<ResponseLogin>> LoginAsync(RequestLoginClient request);
}
