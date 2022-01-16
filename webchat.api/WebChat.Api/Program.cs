using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebChat.Api.Configurations;
using WebChat.Api.Hubs;
using WebChat.Api.Messaging.Consumers;
using WebChat.Application.Services.Security;
using WebChat.Application.Services.Users;
using WebChat.Contracts.Messaging.Stock;
using WebChat.Domain.Users.Interfaces;
using WebChat.Domain.Users.Services;
using WebChat.Infra;
using WebChat.Infra.Repositories;
using WebChat.Messaging;
using WebChat.Security.Domain;
using WebChat.Security.Domain.Configurations;
using WebChat.Security.Domain.Interfaces;
using WebChat.Security.Domain.Tokens.Interfaces;
using WebChat.Security.Domain.Tokens.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(x => x.AddConsole());

builder.Services.AddOptions<JwtSettings>().Bind(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddOptions<MessagingSettings>().Bind(builder.Configuration.GetSection("MessagingSettings"));

builder.Services.AddCors(options =>
{
    options
        .AddPolicy(
            "ChatHubCORS",
            corsBuilder => corsBuilder.WithOrigins(builder.Configuration.GetSection("WebChatWeb:Paths").Get<List<string>>().ToArray())
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed((host) => true));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };

        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddDbContext<WebChatContext>(opt => opt.UseInMemoryDatabase(nameof(WebChatContext)));

#region Microservice DI
builder.Services.AddScoped<ContextMiddleware>();
builder.Services.AddScoped<ILoginFacade, LoginFacade>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddScoped<IUserFacade, UserFacade>();
builder.Services.AddScoped<IUserSearcher, UserSearcher>();
builder.Services.AddScoped<IUserCreator, UserCreator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IBus>(RabbitHutch.CreateBus($"host={builder.Configuration["RabbitMQ:Host"]};virtualHost={builder.Configuration["RabbitMQ:VHost"]};username={builder.Configuration["RabbitMQ:Username"]};password={builder.Configuration["RabbitMQ:Password"]}"));
builder.Services.AddScoped<WebChat.Domain.Messaging.IPublisher, WebChat.Infra.Messaging.Publisher>();

builder.Services.AddTransient<IConsumeAsync<SearchStockQueryResult>, SearchStockQueryResultHandler>();
builder.Services.AddHostedService<MessagingService>(provider =>
{
    var service = new MessagingService(
        provider.GetRequiredService<IBus>(),
        provider.GetRequiredService<IOptions<MessagingSettings>>(),
        provider.GetRequiredService<IServiceScopeFactory>()
    );

    service.Subscribers += subscriber => subscriber.Subscribe<SearchStockQueryResult>();
    return service;
});
#endregion

var app = builder.Build();
var publisher = app.Services.CreateScope().ServiceProvider.GetRequiredService<WebChat.Domain.Messaging.IPublisher>();
while (true)
{
    publisher.PublishAsync(new SearchStockQuery { Code = "aapl.us" }).GetAwaiter().GetResult();
    Thread.Sleep(2000);
}

app.UseCors("ChatHubCORS");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseMiddleware<ContextMiddleware>();
app.MapControllers();

app.UseEndpoints(x =>
{
    x.MapHub<ChatHub>("hubs/chat");
});

app.Run();
