using RabbitMq.Package.Contract;
using RabbitMq.Package.Events;

namespace Consultation.Infrastructure.Queue;

public class RabbitMqEventsDispatcher(IRabbitMqQueueHandler queueHandler) : IRabbitMqEventsDispatcher
{
    private readonly IRabbitMqQueueHandler _queueHandler = queueHandler;

    public async Task SendEvent<T>(IRabbitMqEvent<T> eventToSend)
    {
        await _queueHandler.SendMessage(
            eventToSend.Payload,
            eventToSend.Exchange,
            eventToSend.RoutingKey
        );
    }
}
