using Consultation.Infrastructure.Queue;
using RabbitMq.Package.Events;

namespace Consultation.Application.Events;

public class OnSendEmailConfirmationClientEvent : IRabbitMqEvent<OnSendEmailConfirmationClientMessage>
{
    public string Exchange => RabbitMqConstants.NotificationExchange;

    public string RoutingKey => RabbitMqConstants.NotificationRoutingKey;

    public OnSendEmailConfirmationClientMessage Payload { get; set; }
}