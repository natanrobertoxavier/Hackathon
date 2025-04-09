using FluentValidation;
using User.Communication.Request;

namespace User.Application.UseCase.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<RequestChangePassword>
{
    public ChangePasswordValidator()
    {
        RuleFor(c => c.NewPassword).SetValidator(new PasswordValidator());
    }
}
