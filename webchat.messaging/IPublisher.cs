
namespace WebChat.Messaging;

public interface IPublisher
{
    Task PublishAsync<T>(T message);
}