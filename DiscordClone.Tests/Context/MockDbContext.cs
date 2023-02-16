using DiscordClone.Context;
using DiscordClone.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Tests.Context;

public class MockDbContext
{
    public async Task<MyDbContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        var context = new MyDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "admin",
            PasswordHash = "$2a$11$3GPMCF/SzcFNmohn2n3I6ebWt9s6p00EAWSNfM0mbVR143F/FmOT6"
        });
        context.UserServers.Add(new UserServer
        {
            Id = 1,
            ServerId = 1,
            UserId = 1,
            Role = "admin"
        });
        context.Messages.Add(new Message
        {
            MessageId = 1,
            UserId = 1,
            ServerId = 1,
            Content = "lorem ipsum",
            Date = new DateTime()
        });
        context.Servers.Add(new Server
        {
            ServerId = 1,
            Name = "global chat",
            InviteCode = ""
        });
        context.Servers.Add(new Server
        {
            ServerId = 2,
            Name = "test chat",
            InviteCode = "xdd"
        });
        await context.SaveChangesAsync();
        return context;
    }
}