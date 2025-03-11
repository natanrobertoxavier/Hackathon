using Client.Communication.Response;

namespace Client.Application.UseCase.Recover.RecoverByEmail;

public interface IRecoverByEmailUseCase
{
    Task<Result<ResponseClient>> RecoverByEmailAsync(string email);
}
