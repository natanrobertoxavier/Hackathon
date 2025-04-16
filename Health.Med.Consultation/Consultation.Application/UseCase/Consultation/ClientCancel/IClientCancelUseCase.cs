using Consultation.Communication.Request;
using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.ClientCancel;

public interface IClientCancelUseCase
{
    Task<Result<MessageResult>> ClientCancelConsultationAsync(RequestClientCancel request);
}
