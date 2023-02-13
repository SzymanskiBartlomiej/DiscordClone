using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DiscordClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(MyDbContext context)
        {
            this._context = context;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(string userName , string password)
        {
            //TODO Check if user already exists
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            Console.WriteLine(passwordHash);
            _context.Users.Add(new User
            {
                PasswordHash = passwordHash,
                UserName = userName
            });
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            User user = _context.Users.FirstOrDefault(user => user.UserName == userName);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                IDictionary<string, string> token = new Dictionary<string, string>();
                var exp = DateTime.Now.AddMinutes(15);
                token.Add("token",CreateJwtToken(user));
                token.Add("expiration", exp.ToString());
                token.Add("userID",user.UserId.ToString());
                return Ok(token);
            }

            return BadRequest("Wrong passowrd");
        }

        private string CreateJwtToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.SerialNumber,user.UserId.ToString())
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

        [HttpGet("me")][Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userName = User.Identity.Name;
            var userId = User.FindFirstValue(ClaimTypes.SerialNumber);
            return Ok(new {userName, userId});
        }
    }   
}