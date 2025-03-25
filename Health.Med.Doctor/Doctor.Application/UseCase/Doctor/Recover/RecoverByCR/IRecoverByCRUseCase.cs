using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;

public interface IRecoverByCRUseCase
{
    Task<Result<ResponseDoctor>> RecoverByCRAsync(string cr);
}
