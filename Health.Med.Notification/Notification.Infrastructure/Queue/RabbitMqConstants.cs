namespace Notification.Infrastructure.Queue;
public static class RabbitMqConstants
{
    public const string NotificationExchange = $"notification.exchange";

    public const string NotificationQueueName = "health.med.notification.on-send-email-confirmation-client";
    public const string NotificationRoutingKey = "on-send-email-confirmation-client";
}
