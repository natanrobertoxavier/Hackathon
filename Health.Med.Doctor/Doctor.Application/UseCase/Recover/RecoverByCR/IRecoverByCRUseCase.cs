using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Recover.RecoverByCR;

public interface IRecoverByCRUseCase
{
    Task<Result<ResponseDoctor>> RecoverByCRAsync(string cr);
}
