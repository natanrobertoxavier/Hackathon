using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverByEmail;

public interface IRecoverByEmailUseCase
{
    Task<Result<ResponseDoctor>> RecoverByEmailAsync(string email);
}
