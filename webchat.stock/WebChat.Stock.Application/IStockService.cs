using WebChat.Stock.Domain.StockItems.Entities;

namespace WebChat.Stock.Application;

public interface IStockService
{
    Task<StockItem> GetStockInfoAsync(string itemCode);
}