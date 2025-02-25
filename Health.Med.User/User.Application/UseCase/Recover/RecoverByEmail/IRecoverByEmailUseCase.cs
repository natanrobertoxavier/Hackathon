using User.Communication.Response;

namespace User.Application.UseCase.Recover.RecoverByEmail;

public interface IRecoverByEmailUseCase
{
    Task<Result<ResponseUser>> RecoverByEmailAsync(string email);
}
