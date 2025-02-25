using User.Communication.Request;
using User.Communication.Response;

namespace User.Application.UseCase.ChangePassword;
public interface IChangePasswordUseCase
{
    Task<Result<MessageResult>> ChangePasswordAsync(RequestChangePassword request);
}
