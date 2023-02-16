using System.Security.Claims;
using System.Text.Json;
using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DiscordClone;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly MyDbContext _context;
    private readonly Dictionary<string, string> UserConnectionId = new();

    public ChatHub(MyDbContext context)
    {
        _context = context;
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirstValue(ClaimTypes.SerialNumber);
        var userChats = _context.UserServers
            .Where(server => server.UserId.ToString() == userId)
            .Select(server => server.ServerId)
            .ToList();
        foreach (var serverId in userChats) Groups.AddToGroupAsync(Context.ConnectionId, serverId.ToString());
        UserConnectionId.Add(userId, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User.FindFirstValue(ClaimTypes.SerialNumber);
        UserConnectionId.Remove(userId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task NewMessage(int serverid, string message)
    {
        var userid = int.Parse(Context.User.FindFirstValue(ClaimTypes.SerialNumber));
        var userName = _context.Users.FirstOrDefault(user => user.UserId == userid).UserName;
        _context.Messages.Add(new Message
        {
            UserId = userid,
            ServerId = serverid,
            Content = message,
            Date = DateTime.Now
        });
        await _context.SaveChangesAsync();
        var json = JsonSerializer.Serialize(new
        {
            UserName = userName,
            ServerId = serverid,
            Content = message,
            Date = DateTime.Now
        });

        await Clients.Group(serverid.ToString()).SendAsync("messageReceived", json);
    }

    public async void addToGroup(string userId, string groupName)
    {
        var connectionId = UserConnectionId[userId];
        await Groups.AddToGroupAsync(connectionId, groupName);
    }
}