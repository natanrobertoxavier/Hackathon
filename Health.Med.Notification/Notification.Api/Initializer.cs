using Notification.Api.Listeners;
using Notification.Api.QueueModels;
using Notification.Api.Settings;
using Notification.Infrastructure.Queue;
using RabbitMq.Package.Settings;

namespace Notification.Api;

public static class Initializer
{
    public static void AddConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddRabbitMqService(services, configuration);
        AddRabbitMqSettings(services, configuration);
    }

    private static void AddRabbitMqService(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMqSettings"));
    }

    private static void AddRabbitMqSettings(IServiceCollection services, IConfiguration configuration)
    {
        var config = new RabbitMqSettings();

        configuration.GetSection("RabbitMqSettings").Bind(config);

        services
            .AddQueueHandler(config.ComposedConnectionString)
            .DeclareQueues(
                new RabbitMqQueue(
                    exchangeName: RabbitMqConstants.NotificationExchange,
                    routingKeyName: RabbitMqConstants.NotificationRoutingKey,
                    queueName: RabbitMqConstants.NotificationQueueName)
                )
            ;

        services.AddTransient<QueueListenerBase<SendMailModel>, SendMailListener>();

        services.AddHostedService<QueueListenerHostedService<SendMailModel>>();
    }
}
