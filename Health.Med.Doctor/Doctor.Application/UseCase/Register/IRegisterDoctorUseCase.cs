using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Register;

public interface IRegisterDoctorUseCase
{
    Task<Result<MessageResult>> RegisterDoctorAsync(RequestRegisterDoctor request);
}
