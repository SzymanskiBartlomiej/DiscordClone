using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DiscordClone.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MyDbContext _context;

    public AuthController(MyDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string userName, string password)
    {
        if (CheckIfUserExists(userName)) return BadRequest("User is already registered");
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        _context.Users.Add(new User
        {
            PasswordHash = passwordHash,
            UserName = userName
        });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string userName, string password)
    {
        if (!CheckIfUserExists(userName)) return BadRequest("User not found");
        var user = _context.Users.FirstOrDefault(user => user.UserName == userName);
        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            IDictionary<string, string> token = new Dictionary<string, string>();
            var exp = DateTime.Now.AddMinutes(15);
            token.Add("token", CreateJwtToken(user));
            token.Add("expiration", exp.ToString());
            token.Add("userID", user.UserId.ToString());
            token.Add("userName", user.UserName);
            return Ok(token);
        }

        return BadRequest("Wrong passowrd");
    }

    private string CreateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
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