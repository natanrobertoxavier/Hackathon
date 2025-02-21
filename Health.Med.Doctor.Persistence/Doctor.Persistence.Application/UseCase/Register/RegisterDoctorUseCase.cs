using Doctor.Persistence.Communication.Request;
using Doctor.Persistence.Communication.Response;
using Doctor.Persistence.Domain.Repositories.Contracts;
using Serilog;
using TokenService.Manager.Controller;

namespace Doctor.Persistence.Application.UseCase.Register;
public class RegisterDoctorUseCase(
    IDoctorWriteOnly doctorWriteOnlyrepository,
    IDoctorReadOnly doctorReadOnlyrepository,
    PasswordEncryptor passwordEncryptor,
    ILogger logger) : IRegisterDoctorUseCase
{
    public Task<Result<MessageResult>> RegisterDoctorAsync(RequestRegisterDoctor request)
    {
        throw new NotImplementedException();
    }
}
