using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;

namespace WebChat.Messaging;

public class Publisher : IPublisher
{
    private readonly IBus _bus;
    private readonly IOptions<MessagingSettings> _options;

    public Publisher(IBus bus, IOptions<MessagingSettings> options)
    {
        _bus = bus;
        _options = options;
    }

    public async Task PublishAsync<T>(T message)
    {
        var publications = _options.Value.Exchanges.Select(exchangeName =>
        {
            var exchange = _bus.Advanced.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            return _bus.Advanced.PublishAsync(exchange, string.Empty, true, new Message<T>(message));
        });

        await Task.WhenAll(publications);
    }
}
