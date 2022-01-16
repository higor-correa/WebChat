using WebChat.Application.Services.BOTs.CommandHandlers;

namespace WebChat.Application.Services.BOTs;

public interface ICommandParser
{
    IEnumerable<ICommandHandler> GetHandlers(string message);
    bool IsCommand(string message);
}