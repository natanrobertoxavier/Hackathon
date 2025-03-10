using Client.Communication.Request;
using FluentValidation;

namespace Client.Application.UseCase.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<RequestChangePassword>
{
    public ChangePasswordValidator()
    {
        RuleFor(c => c.NewPassword).SetValidator(new PasswordValidator());
    }
}
