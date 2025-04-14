using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Confirm;

public interface IAcceptUseCase
{
    Task<Result<MessageResult>> AcceptConsultationAsync(Guid consultationId);
}
