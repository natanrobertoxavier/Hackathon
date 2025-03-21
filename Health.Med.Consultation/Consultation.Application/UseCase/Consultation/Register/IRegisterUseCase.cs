using Consultation.Communication.Request;
using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Register;

public interface IRegisterUseCase
{
    Task<Result<MessageResult>> RegisterConsultationAsync(RequestRegisterConsultation request);
}
