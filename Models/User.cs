namespace DiscordClone.Models;

public class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<UserServer> UserServers { get; } = new List<UserServer>();
}