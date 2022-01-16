namespace WebChat.Application.Services.BOTs.CommandHandlers;

public interface ICommandHandler
{
    bool CanHandle(string message);
    Task HandleAsync(string message);
}
