using EasyNetQ.AutoSubscribe;
using WebChat.Contracts.Messaging.Stock;
using WebChat.Messaging;
using WebChat.Stock.Application;

namespace WebChat.Stock.Api.Messaging.Consumers;

public class SearchStockQueryHandler : IConsumeAsync<SearchStockQuery>
{
    private readonly IStockService _stockService;
    private readonly IPublisher _publisher;

    public SearchStockQueryHandler(IStockService stockService, IPublisher publisher)
    {
        _stockService = stockService;
        _publisher = publisher;
    }

    public async Task ConsumeAsync(SearchStockQuery message, CancellationToken cancellationToken = default)
    {
        var stock = await _stockService.GetStockInfoAsync(message.Code);
        await _publisher.PublishAsync(new SearchStockQueryResult { Code = stock.Code, Price = stock.Price });
    }
}
