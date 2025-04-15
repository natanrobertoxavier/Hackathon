using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverBySpecialtyId;

public interface IRecoverBySpecialtyIdUseCase
{
    Task<Result<IEnumerable<ResponseDoctor>>> RecoverBySpecialtyIdAsync(Guid specialtyId);
}
