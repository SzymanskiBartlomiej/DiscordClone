namespace DiscordClone.Models;

public class UserServer
{
    public int Id { get; set; }

    public int ServerId { get; set; }

    public int UserId { get; set; }

    public string? Role { get; set; }

    public virtual Server Server { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}