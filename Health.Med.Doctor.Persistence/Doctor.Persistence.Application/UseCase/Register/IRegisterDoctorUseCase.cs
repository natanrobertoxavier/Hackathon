
using Doctor.Persistence.Communication.Request;
using Doctor.Persistence.Communication.Response;

namespace Doctor.Persistence.Application.UseCase.Register;

public interface IRegisterDoctorUseCase
{
    Task<Result<MessageResult>> RegisterDoctorAsync(RequestRegisterDoctor request);
}
