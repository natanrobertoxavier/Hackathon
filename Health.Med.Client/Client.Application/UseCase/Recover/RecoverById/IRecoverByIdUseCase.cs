using Client.Communication.Response;

namespace Client.Application.UseCase.Recover.RecoverById;

public interface IRecoverByIdUseCase
{
    Task<Result<ResponseClient>> RecoverByIdAsync(Guid clientId);
    Task<Result<ResponseClientBasicInfo>> RecoverBasicInformationByIdAsync(Guid clientId);
}
