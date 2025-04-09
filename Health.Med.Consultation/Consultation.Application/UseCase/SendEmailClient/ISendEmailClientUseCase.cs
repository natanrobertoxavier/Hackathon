using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.Entities.Enum;
using Consultation.Domain.ModelServices;

namespace Consultation.Application.UseCase.SendEmailClient;

public interface ISendEmailClientUseCase
{
    Task<Communication.Response.Result<MessageResult>> SendEmailClientAsync(RequestRegisterConsultation request, DoctorResult doctor, TemplateEmailEnum template);
}
