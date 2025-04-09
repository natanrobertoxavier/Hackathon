using RabbitMq.Package.Events;

namespace Consultation.Infrastructure.Queue;

public interface IRabbitMqEventsDispatcher
{
    Task SendEvent<T>(IRabbitMqEvent<T> eventToSend);
}
