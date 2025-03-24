using MediatR;

namespace Consultation.Domain.Messages;

public abstract class DomainEvent : BaseMessage, INotification
{
    protected DomainEvent() { }
}
