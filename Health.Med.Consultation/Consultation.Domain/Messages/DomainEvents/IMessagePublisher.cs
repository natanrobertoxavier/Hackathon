namespace Consultation.Domain.Messages.DomainEvents;

public interface IMessagePublisher
{
    Task PublishDomainEvent<T>(T domainEvent) where T : DomainEvent;
}
