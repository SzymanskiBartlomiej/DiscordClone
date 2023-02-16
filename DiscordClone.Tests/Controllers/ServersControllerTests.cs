using System.Security.Claims;
using DiscordClone.Controllers;
using DiscordClone.Tests.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.Tests.Controllers;

public class ServersControllerTests
{
    [Fact]
    public async void ServersController_GetUserServersReturnsList()
    {
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new ServersController(context);
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.SerialNumber, "1")
        }));
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = mockUser };
        var res = controller.GetUserServers(1);
        res.Should().Equal(new List<int> { 1 });
    }

    [Fact]
    public async void ServersController_createServer()
    {
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new ServersController(context);
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.SerialNumber, "1")
        }));
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = mockUser };
        await controller.createServer("new server");
        var server = context.Servers.FirstOrDefault(server => server.Name == "new server");
        server.Should().NotBeNull();
    }

    [Fact]
    public async void ServersController_joinServerReturnsOk()
    {
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new ServersController(context);
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.SerialNumber, "1")
        }));
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = mockUser };

        //todo mock signalR connection


        await controller.joinServer("test chat", "xdd");
        var server = context.UserServers.FirstOrDefault(server => server.ServerId == 2 && server.UserId == 1);
        server.Should().NotBeNull();
    }
}