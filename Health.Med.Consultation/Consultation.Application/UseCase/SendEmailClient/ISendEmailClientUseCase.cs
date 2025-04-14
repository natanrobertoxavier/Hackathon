using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.Entities.Enum;
using Consultation.Domain.ModelServices;

namespace Consultation.Application.UseCase.SendEmailClient;

public interface ISendEmailClientUseCase
{
    Task<Communication.Response.Result<MessageResult>> SendEmailSchedulingConsultationClientAsync(RequestRegisterConsultation request, DoctorResult doctor, TemplateEmailEnum template);
    Task<Communication.Response.Result<MessageResult>> SendEmailConfirmationConsultationClientAsync(Guid consultationId, RequestRegisterConsultation request, DoctorResult doctor, TemplateEmailEnum template);
}
