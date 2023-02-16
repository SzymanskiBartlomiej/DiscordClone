using System.Security.Claims;
using System.Text.Json;
using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServersController : ControllerBase
{
    private readonly MyDbContext _context;

    public ServersController(MyDbContext context)
    {
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
        new ChatHub(_context).addToGroup(userId.ToString(), server.ServerId.ToString());
        return Ok();
    }
}