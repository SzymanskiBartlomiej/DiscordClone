using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace DiscordClone.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public AuthController(MyDbContext context, IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string userName, string password)
    {
        if (userName.Length < 1) return BadRequest("UserName should be at least 1 character long");
        
        if (password.Length < 8) return BadRequest("UserName should be at least 8 characters long");
        
        if (!password.Any(char.IsUpper)) return BadRequest("Password should contain at least one capital letter");

        if (!password.Any(char.IsNumber)) return BadRequest("Password should contain at least one number");

        if (!password.Any(char.IsLower)) return BadRequest("Password should contain at least one lower letter");
        
        if (CheckIfUserExists(userName)) return BadRequest("User is already registered");
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        _context.Users.Add(new User
        {
            PasswordHash = passwordHash,
            UserName = userName
        });
        await _context.SaveChangesAsync();
        var userId = _context.Users.FirstOrDefault(user => user.UserName == userName).UserId;
        _context.UserServers.Add(new UserServer
        {
            ServerId = 7,
            UserId = userId,
            Role = "user"
        });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string userName, string password)
    {
        if (!CheckIfUserExists(userName)) return BadRequest("User not found");
        var user = _context.Users.FirstOrDefault(user => user.UserName == userName);
        var servers = JsonSerializer.Serialize(new ServersController(_context,_hubContext).GetUserServersWithAdminRole(user.UserId));
        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            IDictionary<string, string> token = new Dictionary<string, string>();
            var exp = DateTime.Now.AddMinutes(15);
            token.Add("token", CreateJwtToken(user , servers));
            token.Add("expiration", exp.ToString());
            token.Add("userID", user.UserId.ToString());
            token.Add("userName", user.UserName);
            token.Add("AdminServers", servers);
            return Ok(token);
        }

        return BadRequest("Wrong passowrd");
    }

    private string CreateJwtToken(User user , string servers)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, servers),
            new(ClaimTypes.SerialNumber, user.UserId.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("goPbmiquFMcpM1Fx"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds);
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return jwtToken;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var userName = User.Identity.Name;
        var userId = User.FindFirstValue(ClaimTypes.SerialNumber);
        return Ok(new { userName, userId });
    }

    [NonAction]
    public bool CheckIfUserExists(string userName)
    {
        return _context.Users.FirstOrDefault(user => user.UserName == userName) != null;
    }
}