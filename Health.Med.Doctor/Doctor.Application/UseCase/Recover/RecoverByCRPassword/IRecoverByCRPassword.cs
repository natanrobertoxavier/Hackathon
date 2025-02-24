using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Recover.RecoverByCRPassword;

public interface IRecoverByCRPassword
{
    Task<Result<ResponseLogin>> RecoverByCRPasswordAsync(RequestLoginDoctor request);
}
