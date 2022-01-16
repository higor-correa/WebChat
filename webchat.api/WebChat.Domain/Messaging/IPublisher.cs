namespace WebChat.Domain.Messaging;

public interface IPublisher
{
    Task PublishAsync<T>(T message);
}
