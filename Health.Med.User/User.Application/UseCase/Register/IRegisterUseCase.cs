using User.Communication.Request;
using User.Communication.Response;

namespace User.Application.UseCase.Register;

public interface IRegisterUseCase
{
    Task<Result<MessageResult>> RegisterUserAsync(RequestRegisterUser request);
}