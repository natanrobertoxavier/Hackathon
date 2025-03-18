using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notification.Api.Listeners;

public abstract class QueueListenerBase<T> : IDisposable
{
    private readonly string _queueName;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private CancellationTokenSource _cts;
    private Task _listeningTask;

    protected QueueListenerBase(string queueName, IConnectionFactory connectionFactory)
    {
        _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        _connection = connectionFactory?.CreateConnection() ?? throw new ArgumentNullException(nameof(connectionFactory));
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
    }

    protected abstract Task ProcessMessageAsync(T message);

    protected abstract Task ProcessErrorAsync(Exception ex);

    public void StartListening(CancellationToken cancellationToken)
    {
        if (_cts != null)
            throw new InvalidOperationException("The listener is already running.");

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _listeningTask = Task.Run(() =>
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = DeserializeMessage(body);
                    await ProcessMessageAsync(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error processing message: {ex.Message}");
                    await ProcessErrorAsync(ex);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

            while (!_cts.Token.IsCancellationRequested)
            {
                Task.Delay(100, _cts.Token).Wait();
            }
        }, _cts.Token);
    }

    public async Task StopListening()
    {
        if (_cts == null)
            return;

        _cts.Cancel();
        if (_listeningTask != null)
            await _listeningTask;

        Dispose();
    }

    private T DeserializeMessage(byte[] body)
    {
        var messageJson = Encoding.UTF8.GetString(body);
        return JsonSerializer.Deserialize<T>(messageJson);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _cts?.Dispose();
    }
}