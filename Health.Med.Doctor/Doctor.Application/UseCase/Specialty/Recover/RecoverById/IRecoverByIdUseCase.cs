using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Specialty.Recover.RecoverById;

public interface IRecoverByIdUseCase
{
    Task<Result<ResponseSpecialty>> RecoverByIdAsync(Guid id);
}
