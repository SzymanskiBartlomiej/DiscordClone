using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

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
        public async Task<IActionResult> getChat(int chatid, int skip)
        {
            var result = await _context.Messages
                .Where(chat => chat.ChatId == chatid)
                .OrderBy(chat => chat.Date)
                .Skip(skip)
                .Take(5)
                .ToListAsync();
            return Ok(result);
        }

        [HttpGet("user/{userid}")]
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