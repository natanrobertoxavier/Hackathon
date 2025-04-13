using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Confirm;

public interface IConfirmUseCase
{
    Task<Result<MessageResult>> ConfirmConsultationAsync(Guid consultationId);
}
