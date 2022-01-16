using WebChat.Application.Services.BOTs.CommandHandlers;

namespace WebChat.Application.Services.BOTs;

public class CommandParser : ICommandParser
{
    private readonly IEnumerable<ICommandHandler> _commandHandlers;

    public CommandParser(IEnumerable<ICommandHandler> commandHandlers)
    {
        _commandHandlers = commandHandlers;
    }

    public bool IsCommand(string message)
    {
        return _commandHandlers.Any(x => x.CanHandle(message));
    }

    public IEnumerable<ICommandHandler> GetHandlers(string message)
    {
        return _commandHandlers.Where(x => x.CanHandle(message));
    }
}
