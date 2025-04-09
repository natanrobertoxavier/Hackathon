using Consultation.Domain.Messages;
using Consultation.Domain.Messages.DomainEvents;
using MediatR;

namespace Consultation.Application.Messages;

public class MessagePublisher(IMediator mediatr) : IMessagePublisher
{
    private readonly IMediator _mediatr = mediatr;

    public async Task PublishDomainEvent<T>(T domainEvent) where T : DomainEvent
        => await _mediatr.Publish(domainEvent);
}
