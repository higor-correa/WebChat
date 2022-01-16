using EasyNetQ.AutoSubscribe;
using Microsoft.AspNetCore.SignalR;
using WebChat.Api.Hubs;
using WebChat.Contracts.Messaging.Stock;

namespace WebChat.Api.Messaging.Consumers;

public class SearchStockQueryResultHandler : IConsumeAsync<SearchStockQueryResult>
{
    private readonly IHubContext<ChatHub> chatHubContext;

    public SearchStockQueryResultHandler(IHubContext<ChatHub> chatHubContext)
    {
        this.chatHubContext = chatHubContext;
    }

    public async Task ConsumeAsync(SearchStockQueryResult message, CancellationToken cancellationToken = default)
    {
        var stockMessage = $"{message.Code.ToUpper()} quote is ${message.Price} per share";
        await chatHubContext.Clients.All.SendAsync("ReceiveMessage", "Stock BOT", stockMessage, DateTime.UtcNow, cancellationToken);
    }
}
