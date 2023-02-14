using System.Security.Claims;
using DiscordClone.Context;
using DiscordClone.Models;
using DiscordClone.Controllers;
using DiscordClone.Tests.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace DiscordClone.Tests.Controllers;

public class ChatControllerTests
{
    private readonly ITestOutputHelper _output;

    public ChatControllerTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public async void ChatController_GetChat_ReturnsOK()
    {
        var userId = 1;
        var chatId = 1;
        var context = await new MockDbContext().GetDatabaseContext();
        
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.SerialNumber, userId.ToString())
        }));
        
        var controller = new ChatController(context);
        controller.ControllerContext = new ControllerContext();     
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = mockUser };
        var userChats = await controller.GetChat(chatId);
        userChats.Should().NotBeNull();
        var res = (string)(userChats as OkObjectResult).Value;
        var json = JArray.Parse(res);
        Assert.Equal(1,json.Count );
        json.First["Content"].ToString().Should().Be("lorem ipsum");
        json.First["UserName"].ToString().Should().Be("admin");
        json.First["ChatId"].ToString().Should().Be("1");

    }

    [Fact]
    public async void ChatController_GetChat_ReturnsUnauthorized()
    {
        var userId = 99999;
        var chatId = 1;
        var context = await new MockDbContext().GetDatabaseContext();
        
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.SerialNumber, userId.ToString())
        }));
        
        var controller = new ChatController(context);
        controller.ControllerContext = new ControllerContext();     
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = mockUser };
        var result = await controller.GetChat(chatId);
        var unAuthorizedResult = result as UnauthorizedObjectResult;
        unAuthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public async void ChatController_GetUserHistoryReturnsOk()
    {
        var userId = 1;
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new ChatController(context);
        var res = await controller.GetUserHistory(userId, 0);
        List<Message> userHistory = (List<Message>)(res as OkObjectResult).Value;
        userHistory[0].MessageId.Should().Be(1);
        userHistory[0].UserId.Should().Be(1);
        userHistory[0].Content.Should().Be("lorem ipsum");
        userHistory[0].ChatId.Should().Be(1);
    }
}