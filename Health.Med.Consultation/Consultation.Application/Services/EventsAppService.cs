using Consultation.Application.Contracts;
using Consultation.Application.Events;
using Consultation.Domain.Messages.DomainEvents;
using Consultation.Infrastructure.Queue;

namespace Consultation.Application.Services;

public class EventsAppService(
    IRabbitMqEventsDispatcher rabbitMqEventsDispatcher) : IEventAppService
{
    private readonly IRabbitMqEventsDispatcher _rabbitMqEventsDispatcher = rabbitMqEventsDispatcher;

    public async Task SendEmailConfirmationClientEvent(SendEmailClientEvent message, CancellationToken cancellationToken)
    {
        await _rabbitMqEventsDispatcher.SendEvent(new OnSendEmailConfirmationClientEvent()
        {
            Payload = new OnSendEmailConfirmationClientMessage(
                message.Recipients,
                message.Subject,
                message.Body
            )
        });
    }
}
