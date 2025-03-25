using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Doctor.Register;

public interface IRegisterUseCase
{
    Task<Result<MessageResult>> RegisterDoctorAsync(RequestRegisterDoctor request);
}
