using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Topology;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace WebChat.Messaging;

public class MessagingService : IHostedService, ISubscriber, IDisposable
{
    private readonly IBus _bus;
    private readonly IOptions<MessagingSettings> _messagingOptions;
    private readonly IServiceScope _serviceScope;
    private readonly IQueue _queue;
    private CancellationToken _startCancellationToken;
    private readonly List<IDisposable> _consumers;

    public delegate void SubscribeToMessages(ISubscriber subscriber);
    public SubscribeToMessages? Subscribers;

    public MessagingService(IBus bus, IOptions<MessagingSettings> messagingOptions, IServiceScopeFactory serviceScopeFactory)
    {
        _consumers = new List<IDisposable>();
        _startCancellationToken = default;
        _bus = bus;
        _messagingOptions = messagingOptions;
        _serviceScope = serviceScopeFactory.CreateScope();
        _queue = _bus.Advanced.QueueDeclare(_messagingOptions.Value.QueueName);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _startCancellationToken = cancellationToken;

        foreach (var exchangeName in _messagingOptions.Value.Exchanges)
        {
            var exchange = _bus.Advanced.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _bus.Advanced.Bind(exchange, _queue, string.Empty, cancellationToken: cancellationToken);
        }

        Subscribers?.Invoke(this);

        return Task.CompletedTask;
    }

    public ISubscriber Subscribe<T>() where T : class
    {
        _consumers.Add(Consume<T>(_queue, _startCancellationToken));
        return this;
    }

    private IDisposable Consume<T>(IQueue queue, CancellationToken cancellationToken) where T : class
    {
        return _bus.Advanced.Consume<T>(queue, async (message, info) =>
        {
            var logger = _serviceScope.ServiceProvider.GetRequiredService<ILogger<IConsumeAsync<T>>>();
            logger.LogInformation($"Consuming: {message.MessageType.Name}");
            try
            {
                await _serviceScope.ServiceProvider.GetRequiredService<IConsumeAsync<T>>().ConsumeAsync(message.Body, cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error consuming:{message.MessageType.Name}:{Environment.NewLine}{JsonConvert.SerializeObject(message.Body)}");
                throw;
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumers.ForEach(x => x.Dispose());
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
        _bus.Dispose();
    }
}

public interface ISubscriber
{
    ISubscriber Subscribe<T>() where T : class;
}