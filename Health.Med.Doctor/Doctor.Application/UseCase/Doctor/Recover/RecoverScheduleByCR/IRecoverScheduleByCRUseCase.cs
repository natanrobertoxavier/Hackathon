using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverScheduleByCRM;

public interface IRecoverScheduleByCRUseCase
{
    Task<Result<ResponseScheduleDoctor>> RecoverScheduleByCRAsync(string cr);
}
