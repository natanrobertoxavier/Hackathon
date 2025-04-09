using User.Communication.Request;
using User.Communication.Response;

namespace User.Application.UseCase.Login;

public interface ILoginUseCase
{
    Task<Result<ResponseLogin>> LoginAsync(RequestLoginUser request);
}
