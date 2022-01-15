using Microsoft.AspNetCore.SignalR;

namespace WebChat.Api.Hubs;

public class ChatHub : Hub
{
  

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", Context.User?.Identity?.Name, message);
    }
}
