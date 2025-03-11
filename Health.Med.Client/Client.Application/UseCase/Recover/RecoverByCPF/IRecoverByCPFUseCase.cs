using Client.Communication.Response;

namespace Client.Application.UseCase.Recover.RecoverByCPF;

public interface IRecoverByCPFUseCase
{
    Task<Result<ResponseClient>> RecoverByCPFAsync(string email);
}
