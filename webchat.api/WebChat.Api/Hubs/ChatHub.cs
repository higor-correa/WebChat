using Microsoft.AspNetCore.SignalR;
using WebChat.Application.Services.BOTs;

namespace WebChat.Api.Hubs;

public class ChatHub : Hub
{
    private readonly ICommandParser _commandParser;

    public ChatHub(ICommandParser commandParser)
    {
        _commandParser = commandParser;
    }

    public async Task SendMessage(string message)
    {
        if (_commandParser.IsCommand(message))
        {
            var handleResults = _commandParser.GetHandlers(message)
                                              .Select(x => x.HandleAsync(message));
            await Task.WhenAll(handleResults);
        }

        await Clients.All.SendAsync("ReceiveMessage", Context.User?.Identity?.Name ?? "unknown", message, DateTime.UtcNow);
    }
}
