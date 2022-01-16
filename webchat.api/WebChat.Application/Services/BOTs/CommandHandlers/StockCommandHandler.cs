using System.Text.RegularExpressions;
using WebChat.Contracts.Messaging.Stock;
using WebChat.Domain.Messaging;

namespace WebChat.Application.Services.BOTs.CommandHandlers;

public class StockCommandHandler : ICommandHandler
{
    private readonly Regex _commandRegex;
    private readonly IPublisher _publisher;

    public StockCommandHandler(IPublisher publisher)
    {
        _commandRegex = new Regex(@"^\/stock=\S+$", RegexOptions.IgnoreCase);
        _publisher = publisher;
    }

    public bool CanHandle(string message)
    {
        return _commandRegex.IsMatch(message.Trim());
    }

    public Task HandleAsync(string message)
    {
        return _publisher.PublishAsync(new SearchStockQuery { Code = message.Split("=").LastOrDefault(string.Empty) });
    }
}
