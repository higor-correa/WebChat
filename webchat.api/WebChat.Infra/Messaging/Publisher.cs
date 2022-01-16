using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;
using WebChat.Domain.Messaging;

namespace WebChat.Infra.Messaging;

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
        var exchange = _bus.Advanced.ExchangeDeclare(_options.Value.Exchange, ExchangeType.Direct);
        await _bus.Advanced.PublishAsync(exchange, string.Empty, true, new Message<T>(message)); 
    }
}
