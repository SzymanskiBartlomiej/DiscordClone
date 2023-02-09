using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone;

public class ChatHub : Hub
{
    private readonly DbContext _context;
    public ChatHub(DbContext context)
    {
        this._context = context;
    }

    public async Task NewMessage(int chatid, int userid, string message)
    {
        // var data = new Chat
        // {
        //     ChatId = chatid,
        //     UserId = userid,
        //     Messages = message,
        //     Date = DateTime.Now
        // };
        // await Clients.All.SendAsync("messageReceived", data);
        // this._context.Add(data);
    }
}