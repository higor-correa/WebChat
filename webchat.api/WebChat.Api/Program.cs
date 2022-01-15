using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebChat.Api.Hubs;
using WebChat.API.Configurations;
using WebChat.Application.Services.Security;
using WebChat.Application.Services.Users;
using WebChat.Domain.Employees.Interfaces;
using WebChat.Domain.Users.Interfaces;
using WebChat.Domain.Users.Services;
using WebChat.Infra;
using WebChat.Infra.Repositories;
using WebChat.Security.Domain;
using WebChat.Security.Domain.Configurations;
using WebChat.Security.Domain.Interfaces;
using WebChat.Security.Domain.Tokens.Interfaces;
using WebChat.Security.Domain.Tokens.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(x => x.AddConsole());

builder.Services.AddOptions<JwtSettings>().Bind(builder.Configuration.GetSection("JwtSettings"));

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
#endregion

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
app.UseAuthentication();
app.UseMiddleware<ContextMiddleware>();
app.MapControllers();

app.UseEndpoints(x =>
{
    x.MapHub<ChatHub>("hubs/chat");
});

app.Run();
