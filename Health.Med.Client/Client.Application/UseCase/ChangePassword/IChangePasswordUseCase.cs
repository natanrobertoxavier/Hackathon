using Client.Communication.Request;
using Client.Communication.Response;

namespace Client.Application.UseCase.ChangePassword;
public interface IChangePasswordUseCase
{
    Task<Result<MessageResult>> ChangePasswordAsync(RequestChangePassword request);
}
