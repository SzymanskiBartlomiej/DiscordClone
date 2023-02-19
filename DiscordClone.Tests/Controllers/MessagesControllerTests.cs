using System.Security.Claims;
using DiscordClone.Controllers;
using DiscordClone.Models;
using DiscordClone.Tests.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace DiscordClone.Tests.Controllers;

public class MessagesControllerTests
{
    private readonly ITestOutputHelper _output;

    public MessagesControllerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async void ChatController_GetMessages_ReturnsOK()
    {
        var userId = 1;
        var ServerId = 1;
        var context = await new MockDbContext().GetDatabaseContext();
        var mockHubContext = new Mock<IHubContext<ChatHub>>();

        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.SerialNumber, userId.ToString())
        }));

        var controller = new MessagesController(context,mockHubContext.Object);
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = mockUser };
        var messages = await controller.GetMessages(ServerId);
        messages.Should().NotBeNull();
        var res = (string)(messages as OkObjectResult).Value;
        var json = JArray.Parse(res);
        Assert.Equal(1, json.Count);

        json.First["Content"].ToString().Should().Be("lorem ipsum");
        json.First["UserName"].ToString().Should().Be("admin");
        json.First["ServerId"].ToString().Should().Be("1");
    }

    [Fact]
    public async void ChatController_GetMessagesReturnsUnauthorized()
    {
        var userId = 99999;
        var serverId = 1;
        var context = await new MockDbContext().GetDatabaseContext();
        var mockHubContext = new Mock<IHubContext<ChatHub>>();

        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.SerialNumber, userId.ToString())
        }));

        var controller = new MessagesController(context,mockHubContext.Object);
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = mockUser };
        var result = await controller.GetMessages(serverId);
        var unAuthorizedResult = result as UnauthorizedObjectResult;
        unAuthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public async void ChatController_GetUserHistoryReturnsOk()
    {
        var userId = 1;
        var context = await new MockDbContext().GetDatabaseContext();
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var controller = new MessagesController(context,mockHubContext.Object);
        var res = await controller.GetUserHistory(userId, 0);
        var userHistory = (List<Message>)(res as OkObjectResult).Value;
        userHistory[0].MessageId.Should().Be(1);
        userHistory[0].UserId.Should().Be(1);
        userHistory[0].Content.Should().Be("lorem ipsum");
        userHistory[0].ServerId.Should().Be(1);
    }
}