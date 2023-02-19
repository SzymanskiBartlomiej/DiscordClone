using System.Security.Claims;
using System.Text.Json;
using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Controllers;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize]
public class ServersController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;
    public ServersController(MyDbContext context , IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
        _context = context;
    }

    [NonAction]
    public List<int> GetUserServers(int userId)
    {
        var userServers = _context.UserServers
            .Where(server => server.UserId == userId)
            .Select(server => server.ServerId)
            .ToList();
        return userServers;
    }
    
    [NonAction]
    public List<int> GetUserServersWithAdminRole(int userId)
    {
        var userServers = _context.UserServers
            .Where(server => (server.UserId == userId && server.Role == "admin"))
            .Select(server => server.ServerId)
            .ToList();
        return userServers;
    }

    [HttpGet]
    public string GetUserServersNames()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.SerialNumber));
        var userServers = _context.UserServers
            .Where(server => server.UserId == userId)
            .Select(server => new { server.Server.Name, server.ServerId });
        return JsonSerializer.Serialize(userServers);
    }

    [HttpPost("create")]
    public async Task<IActionResult> createServer(string name)
    {
        if (_context.Servers.FirstOrDefault(server => server.Name == name) != null)
            return BadRequest("Server with this name already exists");

        var inviteCode = Guid.NewGuid().ToString();
        _context.Servers.Add(new Server { Name = name, InviteCode = inviteCode });
        await _context.SaveChangesAsync();
        await joinServer(name, inviteCode);
        return Ok();
    }

    [HttpPost("join")]
    public async Task<IActionResult> joinServer(string name, string inviteCode)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.SerialNumber));
        var server = _context.Servers.FirstOrDefault(server => server.Name == name);
        if (server == null) return BadRequest("Server with this name do not exists");
        if (server.InviteCode != inviteCode) return BadRequest("Wrong invite code");

        _context.UserServers.Add(new UserServer
        {
            ServerId = server.ServerId,
            UserId = userId,
            Role = "user"
        });
        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("addToGroup", userId.ToString(), server.ServerId.ToString());
        return Ok();
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetServerMembers(int serverId)
    {
        if (_context.Servers.FirstOrDefault(server => server.ServerId == serverId) == null)
            return BadRequest("This server do not exists");
        var users = await _context.UserServers.Where(userServer => userServer.ServerId == serverId)
            .Select(userServer => userServer.User.UserName)
            .ToListAsync();
        return Ok(users);
    }
}