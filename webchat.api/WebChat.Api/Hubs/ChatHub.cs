﻿using Microsoft.AspNetCore.SignalR;

namespace WebChat.Api.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string channel, string user, string message)
    {
        await Clients.Group(channel).SendAsync("ReceiveMessage", user, message);
    }
}