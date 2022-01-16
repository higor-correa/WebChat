using System.Globalization;
using WebChat.Stock.Domain.StockItems.Entities;
using WebChat.Stock.Domain.StockItems.Interfaces;

namespace WebChat.Stock.Gateway.StockItems;

public class StockItemGateway : IStockItemGateway
{
    private const string STOCK_URI = "/q/l/?s={0}&f=sd2t2ohlcv&h&e=csv";
    private const string CSV_DELIMITER = ",";
    private const int CLOSE_PRICE_INDEX = 6;

    private readonly HttpClient _httpClient;

    public StockItemGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StockItem> GetStockInfoAsync(string itemCode)
    {
        var stockCsvStream = await _httpClient.GetStreamAsync(string.Format(STOCK_URI, itemCode));
        using var streamReader = new StreamReader(stockCsvStream);
        var file = await streamReader.ReadToEndAsync();

        return file.Split(Environment.NewLine)
                   .Skip(1)
                   .Select(x =>
                   {
                       var fields = x.Split(CSV_DELIMITER);
                       return new StockItem { Code = itemCode, Price = decimal.Parse(fields[CLOSE_PRICE_INDEX], CultureInfo.InvariantCulture) };
                   })
                   .FirstOrDefault(new StockItem { Code = itemCode });
    }
}
