using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.Entities.Enum;
using Consultation.Domain.ModelServices;

namespace Consultation.Application.UseCase.SendEmailDoctor;

public interface ISendEmailDoctorUseCase
{
    Task<Communication.Response.Result<MessageResult>> SendEmailDoctorAsync(RequestRegisterConsultation request, DoctorResult doctor, TemplateEmailEnum template);
}
