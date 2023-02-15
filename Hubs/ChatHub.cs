using System.Security.Claims;
using System.Text.Json;
using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace DiscordClone;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly MyDbContext _context;
    public ChatHub(MyDbContext context)
    {
        this._context = context;

    }
    public override Task OnConnectedAsync()
    {
        var userChats = _context.UserServers
            .Where(server => server.UserId.ToString() == Context.User.FindFirstValue(ClaimTypes.SerialNumber))
            .Select(server => server.ServerId)
            .ToList();
        foreach (var serverId in userChats)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, serverId.ToString());
        }
        return base.OnConnectedAsync();
    }
    public async Task NewMessage(int serverid, string message)
    {
        var userid = int.Parse(Context.User.FindFirstValue(ClaimTypes.SerialNumber));
        var userName = _context.Users.FirstOrDefault(user => user.UserId == userid).UserName;
        this._context.Messages.Add(new Message{
            UserId = userid,
            ServerId = serverid,
            Content = message,
            Date = DateTime.Now });
        await this._context.SaveChangesAsync();
        var json = JsonSerializer.Serialize(new
        {
            UserName = userName,
            ServerId = serverid,
            Content = message,
            Date = DateTime.Now
        });

        await Clients.Group(serverid.ToString()).SendAsync("messageReceived", json);
    }
}