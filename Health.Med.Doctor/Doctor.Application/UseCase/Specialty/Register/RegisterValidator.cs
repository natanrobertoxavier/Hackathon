using Doctor.Communication.Request;
using FluentValidation;
using Health.Med.Exceptions;

namespace Doctor.Application.UseCase.Specialty.Register;

public class RegisterValidator : AbstractValidator<RequestRegisterSpecialty>
{
    public RegisterValidator()
    {
        RuleFor(c => c.Description).NotEmpty().WithMessage(ErrorsMessages.BlankDescription);
    }
}
