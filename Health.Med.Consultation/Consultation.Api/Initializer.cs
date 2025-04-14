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
public static class ConsultationTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenFromConsultationLinks(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var path = context.Request.Path;

            if ((path.StartsWithSegments("/api/v1/consultation/accept", out var remaining) ||
                 path.StartsWithSegments("/api/v1/consultation/refuse", out remaining)))
            {
                var segments = remaining.Value.Trim('/').Split('/');

                if (segments.Length == 2)
                {
                    var id = segments[0];
                    var token = segments[1];

                    if (Guid.TryParse(id, out _) && !string.IsNullOrWhiteSpace(token))
                    {
                        context.Request.Headers["Authorization"] = $"Bearer {token}";
                    }
                }
            }

            await next();
        });
    }
}
