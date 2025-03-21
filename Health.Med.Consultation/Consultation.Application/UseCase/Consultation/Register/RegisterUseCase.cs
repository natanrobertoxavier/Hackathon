using Consultation.Communication.Request;
using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Register;

public class RegisterUseCase : IRegisterUseCase
{
    public Task<Result<MessageResult>> RegisterConsultationAsync(RequestRegisterConsultation request)
    {
        throw new NotImplementedException();
    }
}
