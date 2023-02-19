using System.Security.Claims;
using System.Text.Json;
using DiscordClone.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessagesController(MyDbContext context, IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
        _context = context;
    }


    [HttpGet("{serverid}")]
    [Authorize]
    public async Task<IActionResult> GetMessages(int serverid)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.SerialNumber));
        var userServers = new ServersController(_context,_hubContext).GetUserServers(userId);
        if (!userServers.Contains(serverid)) return Unauthorized("You're not part of this chat ;c");
        var result = await _context.Messages
            .Where(message => message.ServerId == serverid)
            .OrderBy(message => message.Date)
            // .Skip(skip)
            .Take(15)
            .Select(message => new { message.Content, message.Date, message.User.UserName, message.ServerId , message.MessageId})
            .ToListAsync();
        return Ok(JsonSerializer.Serialize(result));
    }

    [HttpGet("user/{userid}")]
    [Authorize]
    public async Task<IActionResult> GetUserHistory(int userid, int skip)
    {
        var result = await _context.Messages
            .Where(chat => chat.UserId == userid)
            .OrderBy(chat => chat.Date)
            .Skip(skip)
            .Take(5)
            .ToListAsync();
        return Ok(result);
    }
}