using FluentValidation;
using Health.Med.Exceptions;

namespace Doctor.Application.UseCase;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password).NotEmpty().WithMessage(ErrorsMessages.BlankPassword);
        When(password => !string.IsNullOrWhiteSpace(password), () =>
        {
            RuleFor(password => password.Length).GreaterThanOrEqualTo(6).WithMessage(ErrorsMessages.MinimumSixCharacters);
        });
    }
}