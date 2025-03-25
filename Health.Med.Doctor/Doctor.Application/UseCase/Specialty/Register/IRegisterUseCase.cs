using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Specialty.Register;

public interface IRegisterUseCase
{
    Task<Result<MessageResult>> RegisterSpecialtyAsync(RequestRegisterSpecialty request);
}
