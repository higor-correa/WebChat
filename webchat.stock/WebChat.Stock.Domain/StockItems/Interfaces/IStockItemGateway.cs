using WebChat.Stock.Domain.StockItems.Entities;

namespace WebChat.Stock.Domain.StockItems.Interfaces;

public interface IStockItemGateway
{
    Task<StockItem> GetStockInfoAsync(string itemCode);
}
