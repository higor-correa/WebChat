using WebChat.Stock.Domain.StockItems.Entities;
using WebChat.Stock.Domain.StockItems.Interfaces;

namespace WebChat.Stock.Application;

public class StockService : IStockService
{
    private readonly IStockItemGateway _stockItemGateway;

    public StockService(IStockItemGateway stockItemGateway)
    {
        _stockItemGateway = stockItemGateway;
    }

    public async Task<StockItem> GetStockInfoAsync(string itemCode)
    {
        return await _stockItemGateway.GetStockInfoAsync(itemCode);
    }
}
