using Consultation.Infrastructure.Queue;
using RabbitMq.Package.Settings;

namespace Consultation.Api;

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
    }
}
