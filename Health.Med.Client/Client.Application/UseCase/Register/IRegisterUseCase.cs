using Client.Communication.Request;
using Client.Communication.Response;

namespace Client.Application.UseCase.Register;

public interface IRegisterUseCase
{
    Task<Result<MessageResult>> RegisterClientAsync(RequestRegisterClient request);
}