using Notification.Api.Mapping;
using Notification.Api.QueueModels;
using Notification.Application.UseCase.SendMail;
using Notification.Infrastructure.Queue;
using RabbitMQ.Client;

namespace Notification.Api.Listeners;

public class SendMailListener(
    IConnectionFactory connectionFactory,
    Serilog.ILogger logger,
    IServiceScopeFactory scopeFactory)
    : QueueListenerBase<SendMailModel>(
        RabbitMqConstants.NotificationQueueName,
        connectionFactory)
{
    private readonly Serilog.ILogger _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ProcessMessageAsync(SendMailModel message)
    {
        try
        {
            _logger.Information($"{nameof(SendMailListener)} - Início do processamento da mensagem.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var sendMailUseCase = scope.ServiceProvider.GetRequiredService<ISendMailUseCase>();

                var sendMail = await sendMailUseCase.SendAsync(message.MessageToRequest());

                if (!sendMail.IsSuccess())
                {
                    _logger.Error($"{nameof(SendMailListener)} - Ocorreu um erro ao enviar o e-mail. {sendMail.Errors}");
                }
            }

            _logger.Information($"{nameof(SendMailListener)} - Fim do processamento da mensagem.");
        }
        catch (Exception ex)
        {
            await ProcessErrorAsync(ex);
        }
    }

    protected override async Task ProcessErrorAsync(Exception ex)
    {
        _logger.Error($"Ocorreu um erro durante o processamento da mensagem. {ex.Message}");

        await Task.CompletedTask;
    }
}