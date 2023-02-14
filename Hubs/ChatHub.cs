using System.Security.Claims;
using System.Text.Json;
using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Hub = Microsoft.AspNetCore.SignalR.Hub;
using IUserIdProvider = Microsoft.AspNetCore.SignalR.IUserIdProvider;

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
        var userChats = _context.Servers
            .Where(server => server.UserId.ToString() == Context.User.FindFirstValue(ClaimTypes.SerialNumber))
            .Select(server => server.ChatId)
            .ToList();
        foreach (var chatID in userChats)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, chatID.ToString());
        }
        return base.OnConnectedAsync();
    }
    public async Task NewMessage(int chatid, int userid, string message)
    {
        var userName = _context.Users.FirstOrDefault(user => user.UserId == userid).UserName;
        this._context.Messages.Add(new Message{
            UserId = userid,
            ChatId = chatid,
            Content = message,
            Date = DateTime.Now });
        await this._context.SaveChangesAsync();
        var json = JsonSerializer.Serialize(new
        {
            UserName = userName,
            ChatID = chatid,
            Content = message,
            Date = DateTime.Now
        });

        await Clients.Group(chatid.ToString()).SendAsync("messageReceived", json);
    }
}