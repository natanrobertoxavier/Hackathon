using Notification.Api.Listeners;
using System.Collections.Concurrent;

namespace Notification.Api.Settings;

public class QueueListenerHostedService<T> : IHostedService
{
    private readonly QueueListenerBase<T> _queueListener;
    private readonly ConcurrentBag<Task> _runningListeners = new();

    private readonly CancellationTokenSource _cts = new();

    public QueueListenerHostedService(QueueListenerBase<T> queueListener)
    {
        _queueListener = queueListener;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_queueListener is not null)
        {
            var listenerTask = Task.Run(async () =>
            {
                try
                {
                    _queueListener.StartListening(_cts.Token);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Erro ao iniciar listener: {ex.Message}");
                }
            }, cancellationToken);

            _runningListeners.Add(listenerTask);
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();

        try
        {
            await Task.WhenAll(_runningListeners);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Erro ao parar listeners: {ex.Message}");
        }
    }
}