using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Options;
using WebChat.Contracts.Messaging.Stock;
using WebChat.Messaging;
using WebChat.Stock.Api.Messaging.Consumers;
using WebChat.Stock.Application;
using WebChat.Stock.Domain.StockItems.Interfaces;
using WebChat.Stock.Gateway.StockItems;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MessagingSettings>().Bind(builder.Configuration.GetSection("MessagingSettings"));

builder.Services.AddSingleton<IBus>(RabbitHutch.CreateBus($"host={builder.Configuration["RabbitMQ:Host"]};virtualHost={builder.Configuration["RabbitMQ:VHost"]};username={builder.Configuration["RabbitMQ:Username"]};password={builder.Configuration["RabbitMQ:Password"]}"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Microservice DI
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockItemGateway, StockItemGateway>()
                .AddHttpClient<IStockItemGateway, StockItemGateway>(x => x.BaseAddress = new Uri(builder.Configuration["StockService:BaseUrl"]));

builder.Services.AddScoped<EasyNetQ.AutoSubscribe.IConsumeAsync<SearchStockQuery>, SearchStockQueryHandler>();
builder.Services.AddScoped<IPublisher, Publisher>();
builder.Services.AddHostedService<MessagingService>(provider =>
{
    var consumer = provider.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider.GetRequiredService<IConsumeAsync<SearchStockQuery>>();
    var service = new MessagingService(
        provider.GetRequiredService<IBus>(), 
        provider.GetRequiredService<IOptions<MessagingSettings>>(), 
        provider.GetRequiredService<IServiceScopeFactory>()
    );

    service.Subscribers += subscriber => subscriber.Subscribe<SearchStockQuery>();
    return service;
});

#endregion

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
