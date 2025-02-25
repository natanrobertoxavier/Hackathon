using FluentValidation;
using Health.Med.Exceptions;
using Doctor.Communication.Request;

namespace Doctor.Application.UseCase.Register;

public class RegisterValidator : AbstractValidator<RequestRegisterDoctor>
{
    public RegisterValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage(ErrorsMessages.BlankName);
        RuleFor(c => c.Email).NotEmpty().WithMessage(ErrorsMessages.BlankEmail);
        RuleFor(c => c.CR).NotEmpty().WithMessage(ErrorsMessages.BlankEmail);
        RuleFor(c => c.Password).SetValidator(new PasswordValidator());
        When(c => !string.IsNullOrWhiteSpace(c.Email), () =>
        {
            RuleFor(c => c.Email).EmailAddress().WithMessage(ErrorsMessages.InvalidEmail);
        });
    }
}
