using Consultation.Application.Contracts;
using Consultation.Domain.Messages.DomainEvents;
using MediatR;

namespace Consultation.Application.Messages.Handlers;

public class SendEmailConfirmationClientEventHandler(
    IEventAppService eventAppService) : INotificationHandler<SendEmailClientEvent>
{
    private readonly IEventAppService _eventAppService = eventAppService;

    public async Task Handle(SendEmailClientEvent message, CancellationToken cancellationToken)
    {
        await _eventAppService.SendEmailConfirmationClientEvent(message, cancellationToken);
    }
}
