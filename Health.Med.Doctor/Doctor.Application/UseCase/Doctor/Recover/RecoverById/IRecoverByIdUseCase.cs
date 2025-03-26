using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverById;

public interface IRecoverByIdUseCase
{
    Task<Result<ResponseDoctor>> RecoverByIdAsync(Guid doctorId);
}
