using System.Security.Claims;
using DiscordClone.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DiscordClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ChatController : ControllerBase
    {
        private readonly MyDbContext _context;
        public ChatController(MyDbContext context)
        {
            this._context = context;
        }

        [HttpGet("{chatid}")] 
        [Authorize]
        public async Task<IActionResult> getChat(int chatid, int skip)
        {
            var userId = User.FindFirstValue(ClaimTypes.SerialNumber);
            var userServers = _context.Servers
                .Where(server => server.UserId.ToString() == userId)
                .Select(server => server.ChatId)
                .ToList();
            if (!userServers.Contains(chatid))
            {
                return Unauthorized("You're not part of this chat ;c");
            }
            // var result = await _context.Messages
            //     .Where(chat => chat.ChatId == chatid)
            //     .OrderBy(chat => chat.Date)
            //     .Skip(skip)
            //     .Take(15)
            //     .ToListAsync();
            var result =
                from messages in _context.Messages
                join users in _context.Users
                on messages.UserId equals users.UserId
                select new { messages.Content, messages.Date, users.UserName, messages.ChatId };
            return Ok(result);
        }

        [HttpGet("user/{userid}")]
        [Authorize]
        public async Task<IActionResult> getUserHistory(int userid, int skip)
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
}