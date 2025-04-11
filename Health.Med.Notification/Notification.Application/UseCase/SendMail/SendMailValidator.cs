using FluentValidation;
using Health.Med.Exceptions;
using Notification.Communication.Request;

namespace Notification.Application.UseCase.SendMail;
public class SendMailValidator : AbstractValidator<RequestSendMail>
{
    public SendMailValidator(List<string> recipients)
    {
        ValidateEmail(recipients);
        RuleFor(c => c.Recipients).NotEmpty().WithMessage(ErrorsMessages.BlankRecipient);
        RuleFor(c => c.Subject).NotEmpty().WithMessage(ErrorsMessages.BlankSubject);
        RuleFor(c => c.Body).NotEmpty().WithMessage(ErrorsMessages.BlankBody);
    }

    private void ValidateEmail(List<string> recipients)
    {
        recipients.ForEach(recipient =>
        {
            RuleFor(_ => recipient)
                .NotEmpty().WithMessage(ErrorsMessages.BlankRecipient)
                .EmailAddress().WithMessage(ErrorsMessages.InvalidRecipientEmail);
        });
    }
}
