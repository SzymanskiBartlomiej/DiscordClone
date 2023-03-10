using DiscordClone.Controllers;
using DiscordClone.Tests.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace DiscordClone.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async void AuthController_CheckIfUserExists()
    {
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new AuthController(context,mockHubContext.Object);
        controller.CheckIfUserExists("admin").Should().Be(true);
        controller.CheckIfUserExists("xD").Should().Be(false);
    }


    [Fact]
    public async void AuthController_RegisterReturnsOk()
    {
        var userName = "newUser";
        var password = "Password1";
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new AuthController(context,mockHubContext.Object);
        var result = await controller.Register(userName, password);
        var OkResult = result as OkResult;
        OkResult.StatusCode.Should().Be(200);
        context.Users.FirstOrDefault(user => user.UserName == userName).Should().NotBeNull();
    }

    [Fact]
    public async void AuthController_RegisterReturnsUserIsAlreadyRegistered()
    {
        var userName = "admin";
        var password = "Password1";
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new AuthController(context,mockHubContext.Object);
        var result = await controller.Register(userName, password);
        var badRequest = result as BadRequestObjectResult;
        badRequest.StatusCode.Should().Be(400);
        badRequest.Value.Should().Be("User is already registered");
    }

    [Fact]
    public async void AuthController_LoginReturnsOk()
    {
        var userName = "admin";
        var password = "admin";
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new AuthController(context,mockHubContext.Object);
        var result = await controller.Login(userName, password);
        var okResult = result as OkObjectResult;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async void AuthController_LoginReturnsUserNotFound()
    {
        var userName = "admin321";
        var password = "admin";
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new AuthController(context,mockHubContext.Object);
        var result = await controller.Login(userName, password);
        var badRequest = result as BadRequestObjectResult;
        badRequest.StatusCode.Should().Be(400);
        badRequest.Value.Should().Be("User not found");
    }

    [Fact]
    public async void AuthController_LoginReturnsWrongPassword()
    {
        var userName = "admin";
        var password = "admin321";
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var context = await new MockDbContext().GetDatabaseContext();
        var controller = new AuthController(context,mockHubContext.Object);
        var result = await controller.Login(userName, password);
        var badRequest = result as BadRequestObjectResult;
        badRequest.StatusCode.Should().Be(400);
        badRequest.Value.Should().Be("Wrong passowrd");
    }
}