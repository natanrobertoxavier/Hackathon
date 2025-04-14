using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Confirm;

public interface IAcceptUseCase
{
    Task<Result<MessageResultAcceptConsultation>> AcceptConsultationAsync(Guid consultationId, string token);
}
