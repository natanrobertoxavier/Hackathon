using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Doctor.ChangePassword;

public interface IChangePasswordUseCase
{
    Task<Result<MessageResult>> ChangePasswordAsync(RequestChangePassword request);
}
