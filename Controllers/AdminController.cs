using System.Security.Claims;
using System.Text.Json;
using DiscordClone.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly MyDbContext _context;

    public AdminController(MyDbContext context)
    {
        _context = context;
    }
    
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUser(int serverId, string userName)
    {
        // todo implement bans. Now user can rejoin server after being deleted
        if (!User.FindFirstValue(ClaimTypes.Role).Contains(serverId.ToString()))
        {
            return Unauthorized("You don't have admin privileges on this server");
        }
        var userId = _context.Users.First(user => user.UserName == userName).UserId;
        var entity = _context.UserServers.First(userServer => userServer.UserId == userId);
        _context.UserServers.Remove(entity);
        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpDelete("DeleteMessage")]
    public async Task<IActionResult> DeleteMessage(int serverId, int mesageId)
    {
        if (!User.FindFirstValue(ClaimTypes.Role).Contains(serverId.ToString()))
        {
            return Unauthorized("You don't have admin privileges on this server");
        }
        var entity = _context.Messages.First(message => message.MessageId == mesageId);
        _context.Messages.Remove(entity);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("inviteCode")]
    public async Task<IActionResult> GetInviteCode(int serverId)
    {
        if (!User.FindFirstValue(ClaimTypes.Role).Contains(serverId.ToString()))
        {
            return Unauthorized("You don't have admin privileges on this server");
        }

        var inviteCode = _context.Servers.First(server => server.ServerId == serverId).InviteCode;
        return Ok(JsonSerializer.Serialize(inviteCode));
    }

    [HttpPatch("addAdmin")]
    public async Task<IActionResult> AddAdmin(string userName, int serverId)
    {
        if (!User.FindFirstValue(ClaimTypes.Role).Contains(serverId.ToString()))
        {
            return Unauthorized("You don't have admin privileges on this server");
        }

        var entity = _context.UserServers.
            First(userServer => (userServer.ServerId == serverId && userServer.User.UserName == userName));
        if(entity.Role == "admin") return BadRequest("User already has admin privilleges");
        entity.Role = "admin";
        _context.UserServers.Update(entity);
        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpDelete("removeAdmin")]
    public async Task<IActionResult> RemoveAdmin(int userId, int serverId)
    {
        if (!User.FindFirstValue(ClaimTypes.Role).Contains(serverId.ToString()))
        {
            return Unauthorized("You don't have admin privileges on this server");
        }

        var entity = _context.UserServers.First(userServer => (userServer.ServerId == serverId && userServer.UserId == userId));
        entity.Role = "user";
        _context.UserServers.Update(entity);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("listAdmins")]
    public async Task<IActionResult> ListAdmins(int serverId)
    {
        if (!User.FindFirstValue(ClaimTypes.Role).Contains(serverId.ToString()))
        {
            return Unauthorized("You don't have admin privileges on this server");
        }

        var admins = await _context.UserServers.
            Where(userServer => (userServer.ServerId == serverId && userServer.Role == "admin"))
            .Select(userServer => userServer.User.UserName)
            .ToListAsync();
        return Ok(admins);
    }
}