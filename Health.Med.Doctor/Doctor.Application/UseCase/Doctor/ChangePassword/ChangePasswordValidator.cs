using Doctor.Communication.Request;
using FluentValidation;

namespace Doctor.Application.UseCase.Doctor.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<RequestChangePassword>
{
    public ChangePasswordValidator()
    {
        RuleFor(c => c.NewPassword).SetValidator(new PasswordValidator());
    }
}
