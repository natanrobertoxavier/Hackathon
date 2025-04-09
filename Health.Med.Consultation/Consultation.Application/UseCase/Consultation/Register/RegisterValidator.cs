using Consultation.Communication.Request;
using FluentValidation;
using Health.Med.Exceptions;

namespace Consultation.Application.UseCase.Consultation.Register;

public class RegisterValidator : AbstractValidator<RequestRegisterConsultation>
{
    public RegisterValidator()
    {
        RuleFor(c => c.DoctorId).NotEmpty().WithMessage(ErrorsMessages.BlankName);
        RuleFor(c => c.ConsultationDate).NotEmpty().WithMessage(ErrorsMessages.BlankEmail);;
    }
}
