using FluentValidation;
using Health.Med.Exceptions;
using User.Communication.Request;

namespace User.Application.UseCase.Register;

public class RegisterValidator : AbstractValidator<RequestRegisterUser>
{
    public RegisterValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage(ErrorsMessages.BlankName);
        RuleFor(c => c.Email).NotEmpty().WithMessage(ErrorsMessages.BlankEmail);
        RuleFor(c => c.Password).SetValidator(new PasswordValidator());
        When(c => !string.IsNullOrWhiteSpace(c.Email), () =>
        {
            RuleFor(c => c.Email).EmailAddress().WithMessage(ErrorsMessages.InvalidEmail);
        });
    }
}