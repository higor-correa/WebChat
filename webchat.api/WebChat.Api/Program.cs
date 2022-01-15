using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WebChat.Api.Hubs;
using WebChat.API.Configurations;
using WebChat.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(x => x.AddConsole());

builder.Services.AddCors(options =>
{
    options
        .AddPolicy("ChatHubCORS", builder => builder.WithOrigins("http://localhost:3000/")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed((host) => true));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
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

builder.Services.AddDbContext<WebChatContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

var app = builder.Build();

app.UseCors("ChatHubCORS");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<ContextMiddleware>();
app.MapControllers();

app.UseEndpoints(x =>
{
    x.MapHub<ChatHub>("hubs/chat");
});

app.Run();
