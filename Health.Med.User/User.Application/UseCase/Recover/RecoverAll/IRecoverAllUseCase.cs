using User.Communication.Response;

namespace User.Application.UseCase.Recover.RecoverAll;

public interface IRecoverAllUseCase
{
    Task<Result<IEnumerable<ResponseUser>>> RecoverAllAsync(int page, int pageSize);
}
