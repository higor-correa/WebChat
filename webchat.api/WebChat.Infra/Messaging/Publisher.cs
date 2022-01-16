using EasyNetQ;
using Microsoft.Extensions.Options;
using WebChat.Messaging;

namespace WebChat.Infra.Messaging;

public class Publisher : WebChat.Messaging.Publisher, Domain.Messaging.IPublisher
{
    public Publisher(IBus bus, IOptions<MessagingSettings> options) : base(bus, options)
    {
    }
}
