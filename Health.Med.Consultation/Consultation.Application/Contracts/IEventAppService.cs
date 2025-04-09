using Consultation.Domain.Messages.DomainEvents;

namespace Consultation.Application.Contracts;

public interface IEventAppService
{
    Task SendEmailConfirmationClientEvent(SendEmailClientEvent message, CancellationToken cancellationToken);
}
