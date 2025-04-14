using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Refuse;

public interface IRefuseUseCase
{
    Task<Result<MessageResult>> RefuseConsultationAsync(Guid id);
}
