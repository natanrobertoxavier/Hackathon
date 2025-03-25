using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverAll;

public interface IRecoverAllUseCase
{
    Task<Result<IEnumerable<ResponseDoctor>>> RecoverAllAsync(int page, int pageSize);
}
