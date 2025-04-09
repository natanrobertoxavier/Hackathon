using Client.Communication.Response;

namespace Client.Application.UseCase.Recover.RecoverAll;

public interface IRecoverAllUseCase
{
    Task<Result<IEnumerable<ResponseClient>>> RecoverAllAsync(int page, int pageSize);
}
