using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.Entities.Enum;

namespace Consultation.Application.UseCase.SendEmailClient;

public interface ISendEmailClientUseCase
{
    Task<Result<MessageResult>> SendEmailClientAsync(RequestRegisterConsultation request, TemplateEmailEnum template);
}
